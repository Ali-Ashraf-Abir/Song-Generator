using MeltySynth;
using NAudio.Wave;

namespace backend.MusicEngine.Rendering;

/// <summary>
/// Renders a MIDI file to a WAV byte array. DryWetMidi owns MIDI
/// construction (it has the richer, more ergonomic object model used by
/// <see cref="MidiBuilder"/>); MeltySynth owns playback/synthesis (it's a
/// self-contained SoundFont synthesizer with no native dependencies,
/// which matters for cross-platform deployment). The two libraries don't
/// share a MIDI object model, so the bridge between them is the
/// standard MIDI file byte stream itself: DryWetMidi writes standard SMF
/// bytes, MeltySynth's own lightweight parser reads the same bytes back.
/// </summary>
public sealed class WavRenderer : IWavRenderer
{
    private const int SampleRate = 44100;

    private readonly Synthesizer _synthesizer;

    // MeltySynth's Synthesizer/MidiFileSequencer carry internal voice state
    // that Render() mutates; nothing in the library documents concurrent
    // Render calls on one instance as safe. Rather than reload the
    // SoundFont (often tens of MB) on every request, we keep one shared
    // Synthesizer and serialize access to it with this semaphore. Render
    // throughput under concurrent requests is therefore bounded by this
    // single critical section — acceptable for a cache-backed preview
    // endpoint where most requests are cache hits anyway.
    private readonly SemaphoreSlim _renderLock = new(1, 1);

    public WavRenderer(string soundFontPath)
    {
        if (!File.Exists(soundFontPath))
        {
            throw new FileNotFoundException(
                $"SoundFont not found at '{soundFontPath}'. A .sf2 file is required " +
                "for MeltySynth to render audio; see README for where to obtain one.",
                soundFontPath);
        }

        _synthesizer = new Synthesizer(soundFontPath, SampleRate);
    }

    public byte[] Render(Melanchall.DryWetMidi.Core.MidiFile dryWetMidiFile)
    {
        using var midiStream = new MemoryStream();
        dryWetMidiFile.Write(midiStream, Melanchall.DryWetMidi.Core.MidiFileFormat.MultiTrack);
        midiStream.Position = 0;

        var meltyMidiFile = new MeltySynth.MidiFile(midiStream);

        _renderLock.Wait();
        try
        {
            _synthesizer.Reset();

            var sequencer = new MidiFileSequencer(_synthesizer);
            sequencer.Play(meltyMidiFile, loop: false);

            var totalSeconds = meltyMidiFile.Length.TotalSeconds;
            var totalSamples = (int)Math.Ceiling(totalSeconds * SampleRate) + SampleRate; // +1s tail for release/reverb

            var left = new float[totalSamples];
            var right = new float[totalSamples];

            const int blockSize = 4096;
            var offset = 0;
            while (offset < totalSamples)
            {
                var count = Math.Min(blockSize, totalSamples - offset);
                var leftBlock = new Span<float>(left, offset, count);
                var rightBlock = new Span<float>(right, offset, count);

                sequencer.Render(leftBlock, rightBlock);

                offset += count;
            }

            return EncodeToWav(left, right, SampleRate);
        }
        finally
        {
            _renderLock.Release();
        }
    }

    private static byte[] EncodeToWav(float[] left, float[] right, int sampleRate)
    {
        var waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, 2);

        using var memoryStream = new MemoryStream();
        using (var writer = new WaveFileWriter(new IgnoreDisposeStream(memoryStream), waveFormat))
        {
            var interleaved = new float[left.Length * 2];
            for (var i = 0; i < left.Length; i++)
            {
                interleaved[i * 2] = Math.Clamp(left[i], -1f, 1f);
                interleaved[(i * 2) + 1] = Math.Clamp(right[i], -1f, 1f);
            }

            writer.WriteSamples(interleaved, 0, interleaved.Length);
        }

        return memoryStream.ToArray();
    }
}

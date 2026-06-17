namespace backend.MusicEngine.Rendering;

public interface IWavRenderer
{
    /// <summary>
    /// Synthesizes a DryWetMidi <see cref="Melanchall.DryWetMidi.Core.MidiFile"/>
    /// into PCM WAV bytes using a loaded SoundFont, via MeltySynth, then
    /// encodes via NAudio.
    /// </summary>
    byte[] Render(Melanchall.DryWetMidi.Core.MidiFile midiFile);
}

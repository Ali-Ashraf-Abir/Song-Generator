using backend.MusicEngine.Models;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

namespace backend.MusicEngine.Rendering;

using CompositionNoteEvent = backend.MusicEngine.Models.NoteEvent;

public sealed class MidiBuilder : IMidiBuilder
{
    private const short TicksPerQuarterNote = 480;

    private static readonly FourBitNumber MelodyChannel = (FourBitNumber)0;
    private static readonly FourBitNumber ChordChannel = (FourBitNumber)1;
    private static readonly FourBitNumber BassChannel = (FourBitNumber)2;
    private static readonly FourBitNumber DrumChannel = (FourBitNumber)9; // GM percussion channel (10, zero-indexed)

    public MidiFile Build(CompositionResult composition)
    {
        var midiFile = new MidiFile
        {
            TimeDivision = new TicksPerQuarterNoteTimeDivision(TicksPerQuarterNote)
        };

        var tempoTrack = new TrackChunk();
        using (var tempoManager = tempoTrack.ManageTimedEvents())
        {
            var microsecondsPerQuarterNote = (long)Math.Round(60_000_000.0 / composition.Tempo);
            tempoManager.Objects.Add(new TimedEvent(new SetTempoEvent(microsecondsPerQuarterNote)));
        }

        var melodyTrack = BuildInstrumentTrack(
            composition.MelodyNotes,
            MelodyChannel,
            (SevenBitNumber)composition.Profile.Instruments.MelodyProgram,
            TicksPerQuarterNote);

        var chordTrack = BuildInstrumentTrack(
            composition.ChordNotes,
            ChordChannel,
            (SevenBitNumber)composition.Profile.Instruments.ChordProgram,
            TicksPerQuarterNote);

        var bassTrack = BuildInstrumentTrack(
            composition.BassNotes,
            BassChannel,
            (SevenBitNumber)composition.Profile.Instruments.BassProgram,
            TicksPerQuarterNote);

        var drumTrack = BuildDrumTrack(composition.DrumHits, TicksPerQuarterNote);

        midiFile.Chunks.Add(tempoTrack);
        midiFile.Chunks.Add(melodyTrack);
        midiFile.Chunks.Add(chordTrack);
        midiFile.Chunks.Add(bassTrack);
        midiFile.Chunks.Add(drumTrack);

        return midiFile;
    }

    private static TrackChunk BuildInstrumentTrack(
        IReadOnlyList<CompositionNoteEvent> noteEvents,
            FourBitNumber channel,
            SevenBitNumber program,
            short ticksPerQuarterNote)
    {
        var track = new TrackChunk();

        using (var eventsManager = track.ManageTimedEvents())
        {
            eventsManager.Objects.Add(new TimedEvent(
                new ProgramChangeEvent(program) { Channel = channel },
                0));
        }

        using (var notesManager = track.ManageNotes())
        {
            foreach (var noteEvent in noteEvents)
            {
                if (noteEvent.MidiNote < 0 || noteEvent.MidiNote > 127)
                {
                    continue;
                }

                var startTime = BeatsToTicks(noteEvent.StartBeat, ticksPerQuarterNote);
                var lengthTicks = BeatsToTicks(noteEvent.DurationBeats, ticksPerQuarterNote);
                lengthTicks = Math.Max(lengthTicks, 1);

                var note = new Note(
                    (SevenBitNumber)Math.Clamp(noteEvent.MidiNote, 0, 127),
                    lengthTicks,
                    startTime)
                {
                    Channel = channel,
                    Velocity = (SevenBitNumber)Math.Clamp(noteEvent.Velocity, 1, 127),
                    OffVelocity = (SevenBitNumber)0
                };

                notesManager.Objects.Add(note);
            }
        }

        return track;
    }

    private static TrackChunk BuildDrumTrack(
        IReadOnlyList<DrumHitEvent> drumHits,
        short ticksPerQuarterNote)
    {
        var track = new TrackChunk();


        using (var eventsManager = track.ManageTimedEvents())
        {
            eventsManager.Objects.Add(new TimedEvent(
                new ProgramChangeEvent((SevenBitNumber)0) { Channel = DrumChannel },
                0));
        }

        using (var notesManager = track.ManageNotes())
        {
            const long drumHitLengthTicks = 60; // short, percussive

            foreach (var hit in drumHits)
            {
                var startTime = BeatsToTicks(hit.StartBeat, ticksPerQuarterNote);

                var note = new Note(
                    (SevenBitNumber)Math.Clamp(hit.GmKey, 0, 127),
                    drumHitLengthTicks,
                    startTime)
                {
                    Channel = DrumChannel,
                    Velocity = (SevenBitNumber)Math.Clamp(hit.Velocity, 1, 127),
                    OffVelocity = (SevenBitNumber)0
                };

                notesManager.Objects.Add(note);
            }
        }

        return track;
    }

    private static long BeatsToTicks(double beats, short ticksPerQuarterNote)
        => (long)Math.Round(beats * ticksPerQuarterNote);
}

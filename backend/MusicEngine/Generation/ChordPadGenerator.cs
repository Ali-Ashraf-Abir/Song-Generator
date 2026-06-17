using backend.MusicEngine.Models;
using backend.MusicEngine.Theory;

namespace backend.MusicEngine.Generation;


public static class ChordPadGenerator
{
    public static IReadOnlyList<NoteEvent> Generate(
        GenreProfile profile,
        MusicKey key,
        IReadOnlyList<Chord> chordProgression,
        int songSeed,
        int beatsPerBar)
    {
        var notes = new List<NoteEvent>();
        var anchorMidiNote = key.MidiNoteForDegree(0, profile.MelodyOctave - 1);

        var velocityRng = new Random(HashCode.Combine(songSeed, "chord-pad-velocity"));

        for (var bar = 0; bar < chordProgression.Count; bar++)
        {
            var chord = chordProgression[bar];
            var startBeat = bar * beatsPerBar;

            for (var toneIndex = 0; toneIndex < chord.TonePitchClasses.Count; toneIndex++)
            {
                var pitch = chord.ToneNearOctave(toneIndex, anchorMidiNote);
                var velocity = Math.Clamp(48 + velocityRng.Next(-5, 6), 1, 127);

                notes.Add(new NoteEvent
                {
                    MidiNote = pitch,
                    StartBeat = startBeat,
                    DurationBeats = beatsPerBar * 0.92,
                    Velocity = velocity
                });
            }
        }

        return notes;
    }
}

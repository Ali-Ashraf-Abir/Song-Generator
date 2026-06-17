using backend.MusicEngine.Models;
using backend.MusicEngine.Theory;

namespace backend.MusicEngine.Generation;


public static class BasslineGenerator
{
    private static readonly double[][] RhythmPatterns =
    {
        new[] { 4.0 },
        new[] { 2.0, 2.0 },
        new[] { 1.0, 1.0, 1.0, 1.0 },
        new[] { 1.0, 1.0, 2.0 },
        new[] { 0.5, 0.5, 1.0, 1.0, 1.0 },
        new[] { 1.0, 0.5, 0.5, 2.0 }
    };

    public static IReadOnlyList<NoteEvent> Generate(
        GenreProfile profile,
        MusicKey key,
        IReadOnlyList<Chord> chordProgression,
        int songSeed,
        int beatsPerBar)
    {
        var notes = new List<NoteEvent>();
        var anchorMidiNote = key.MidiNoteForDegree(0, profile.BassOctave);

        for (var bar = 0; bar < chordProgression.Count; bar++)
        {
            var chord = chordProgression[bar];

            var rhythmRng = new Random(HashCode.Combine(songSeed, "bass-rhythm", bar));
            var pattern = RhythmPatterns[rhythmRng.Next(RhythmPatterns.Length)];

            var pitchRng = new Random(HashCode.Combine(songSeed, "bass-pitch", bar));
            var velocityRng = new Random(HashCode.Combine(songSeed, "bass-velocity", bar));
            var syncopationRng = new Random(HashCode.Combine(songSeed, "bass-syncopation", bar));

            var beatCursor = 0.0;

            for (var noteIndex = 0; noteIndex < pattern.Length; noteIndex++)
            {
                var duration = pattern[noteIndex];
                var startBeat = (bar * beatsPerBar) + beatCursor;


                int pitch;
                if (noteIndex == 0)
                {
                    pitch = chord.ToneNearOctave(0, anchorMidiNote);
                }
                else if (syncopationRng.NextDouble() < profile.SyncopationBias * 0.3)
                {
                    pitch = key.SnapToScale(chord.ToneNearOctave(0, anchorMidiNote) - 1);
                }
                else
                {
                    var toneChoice = pitchRng.NextDouble() < 0.7 ? 0 : 2 % chord.TonePitchClasses.Count;
                    pitch = chord.ToneNearOctave(toneChoice, anchorMidiNote);
                }

                var velocity = Math.Clamp(78 + velocityRng.Next(-6, 10), 1, 127);

                notes.Add(new NoteEvent
                {
                    MidiNote = pitch,
                    StartBeat = startBeat,
                    DurationBeats = duration * 0.9,
                    Velocity = velocity
                });

                beatCursor += duration;
            }
        }

        return notes;
    }
}

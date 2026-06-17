using backend.MusicEngine.Models;
using backend.MusicEngine.Theory;

namespace backend.MusicEngine.Generation;

public static class MelodyGenerator
{
    private static readonly double[][] RhythmPatterns =
    {
        new[] { 4.0 },                         
        new[] { 2.0, 2.0 },                   
        new[] { 2.0, 1.0, 1.0 },
        new[] { 1.0, 1.0, 2.0 },
        new[] { 1.0, 1.0, 1.0, 1.0 },         
        new[] { 1.5, 1.5, 1.0 },
        new[] { 0.5, 0.5, 1.0, 1.0, 1.0 },     
        new[] { 1.0, 0.5, 0.5, 1.0, 1.0 }
    };

    public static IReadOnlyList<NoteEvent> Generate(
        GenreProfile profile,
        MusicKey key,
        IReadOnlyList<Chord> chordProgression,
        int songSeed,
        int beatsPerBar)
    {
        var notes = new List<NoteEvent>();
        var anchorMidiNote = key.MidiNoteForDegree(0, profile.MelodyOctave);
        var previousPitch = anchorMidiNote;

        for (var bar = 0; bar < chordProgression.Count; bar++)
        {
            var chord = chordProgression[bar];

            var rhythmRng = new Random(HashCode.Combine(songSeed, "melody-rhythm", bar));
            var pattern = RhythmPatterns[rhythmRng.Next(RhythmPatterns.Length)];

            var pitchRng = new Random(HashCode.Combine(songSeed, "melody-pitch", bar));
            var velocityRng = new Random(HashCode.Combine(songSeed, "melody-velocity", bar));
            var restRng = new Random(HashCode.Combine(songSeed, "melody-rest", bar));

            var beatCursor = 0.0;

            for (var noteIndex = 0; noteIndex < pattern.Length; noteIndex++)
            {
                var duration = pattern[noteIndex];
                var startBeat = (bar * beatsPerBar) + beatCursor;


                var isRest = noteIndex > 0 && restRng.NextDouble() < 0.12;

                if (!isRest)
                {
                    var isStrongBeat = noteIndex == 0 || Math.Abs(beatCursor % (beatsPerBar / 2.0)) < 0.01;

                    int pitch;
                    if (isStrongBeat)
                    {
                        var toneIndex = pitchRng.Next(chord.TonePitchClasses.Count);
                        pitch = chord.ToneNearOctave(toneIndex, previousPitch);
                    }
                    else
                    {

                        var step = pitchRng.Next(-2, 3);
                        pitch = key.SnapToScale(previousPitch + step);
                    }

    
                    pitch = ClampToRange(pitch, anchorMidiNote - 7, anchorMidiNote + 12);

                    var velocity = 70 + velocityRng.Next(-8, 18) + (isStrongBeat ? 8 : 0);
                    velocity = Math.Clamp(velocity, 1, 127);

                    notes.Add(new NoteEvent
                    {
                        MidiNote = pitch,
                        StartBeat = startBeat,
                        DurationBeats = duration * 0.95, 
                        Velocity = velocity
                    });

                    previousPitch = pitch;
                }

                beatCursor += duration;
            }
        }

        return notes;
    }

    private static int ClampToRange(int pitch, int min, int max)
    {
        while (pitch < min) pitch += 12;
        while (pitch > max) pitch -= 12;
        return pitch;
    }
}

using backend.MusicEngine.Models;
using backend.MusicEngine.Theory;

namespace backend.MusicEngine.Generation;

public static class ChordProgressionGenerator
{
    public static IReadOnlyList<Chord> Generate(
        GenreProfile profile,
        MusicKey key,
        int songSeed,
        int totalBars)
    {
        var pickRng = new Random(HashCode.Combine(songSeed, "progression-pick"));
        var pattern = profile.ProgressionPool[pickRng.Next(profile.ProgressionPool.Count)];

        var seventhRng = new Random(HashCode.Combine(songSeed, "progression-sevenths"));

        var chords = new List<Chord>(totalBars);

        for (var bar = 0; bar < totalBars; bar++)
        {
            var degree = pattern[bar % pattern.Length];
            var rootPitchClass = key.PitchClassForDegree(degree);
            var quality = key.DiatonicQualityForDegree(degree);

            if (seventhRng.NextDouble() < profile.SeventhChordBias)
            {
                quality = UpgradeToSeventh(quality);
            }

            chords.Add(new Chord(rootPitchClass, quality, degree));
        }

        return chords;
    }

    private static ChordQuality UpgradeToSeventh(ChordQuality triad) => triad switch
    {
        ChordQuality.Major => ChordQuality.Major7,
        ChordQuality.Minor => ChordQuality.Minor7,
        ChordQuality.Diminished => ChordQuality.HalfDiminished7,
        _ => triad
    };
}

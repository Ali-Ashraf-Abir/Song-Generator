using backend.MusicEngine.Models;
using backend.MusicEngine.Theory;

namespace backend.MusicEngine.Generation;

public static class KeyTempoSelector
{
    public static MusicKey SelectKey(GenreProfile profile, int songSeed)
    {
        var rng = new Random(HashCode.Combine(songSeed, "key"));

        var tonicPitchClass = rng.Next(12);
        var scale = profile.PossibleScales[rng.Next(profile.PossibleScales.Count)];

        return new MusicKey(tonicPitchClass, scale);
    }

    public static int SelectTempo(GenreProfile profile, int songSeed)
    {
        var rng = new Random(HashCode.Combine(songSeed, "tempo"));
        return rng.Next(profile.TempoRange.Min, profile.TempoRange.Max + 1);
    }
}

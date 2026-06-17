using backend.MusicEngine.Theory;

namespace backend.MusicEngine.Models;

public enum MusicGenre
{
    Rock,
    Pop,
    Electronic,
    Jazz
}


public sealed class InstrumentSet
{
    public int MelodyProgram { get; init; }
    public int ChordProgram { get; init; }
    public int BassProgram { get; init; }

    public int DrumChannel { get; init; } = 9;
}

public sealed class GenreProfile
{
    public required MusicGenre Genre { get; init; }
    public required (int Min, int Max) TempoRange { get; init; }
    public required IReadOnlyList<ScaleType> PossibleScales { get; init; }
    public required IReadOnlyList<int[]> ProgressionPool { get; init; }
    public required InstrumentSet Instruments { get; init; }
    public required IReadOnlyList<DrumPatternTemplate> DrumPatternPool { get; init; }
    public required double SwingAmount { get; init; }
    public required double SeventhChordBias { get; init; }
    public required double SyncopationBias { get; init; }
    public required int MelodyOctave { get; init; }
    public required int BassOctave { get; init; }
}

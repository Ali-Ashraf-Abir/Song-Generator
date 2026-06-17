using backend.MusicEngine.Theory;

namespace backend.MusicEngine.Models;

public enum MusicGenre
{
    Rock,
    Pop,
    Electronic,
    Jazz
}

/// <summary>
/// General MIDI program numbers (0-indexed) for the instrument roles the
/// engine fills in. Kept as a small struct rather than scattering magic
/// numbers through the generators.
/// </summary>
public sealed class InstrumentSet
{
    public int MelodyProgram { get; init; }
    public int ChordProgram { get; init; }
    public int BassProgram { get; init; }

    // General MIDI percussion is always channel 10 (index 9) and doesn't
    // need a program change, but we keep drum channel config here too.
    public int DrumChannel { get; init; } = 9;
}

/// <summary>
/// All the genre-specific knobs the generators read from. One profile per
/// <see cref="MusicGenre"/>, built once at startup by
/// <see cref="GenreProfileCatalog"/> and treated as immutable/shared data
/// (never mutated per-request) so it's safe under concurrent requests.
/// </summary>
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

namespace backend.MusicEngine.Theory;

/// <summary>
/// Semitone offsets from the tonic for a scale, plus helpers for
/// scale-degree -> chord-quality mapping. All theory in this file is
/// pure and deterministic: same inputs always produce the same outputs.
/// </summary>
public enum ScaleType
{
    Major,
    NaturalMinor,
    Dorian,
    Mixolydian,
    MinorPentatonic,
    HarmonicMinor
}

public enum ChordQuality
{
    Major,
    Minor,
    Diminished,
    Augmented,
    Dominant7,
    Major7,
    Minor7,
    HalfDiminished7
}

/// <summary>
/// A musical key: a tonic pitch class (0-11, where 0 = C) plus a scale type.
/// Provides scale-degree lookups used by chord/melody/bass generators.
/// </summary>
public sealed class MusicKey
{
    public int TonicPitchClass { get; }
    public ScaleType Scale { get; }
    private readonly int[] _intervals;

    public MusicKey(int tonicPitchClass, ScaleType scale)
    {
        TonicPitchClass = ((tonicPitchClass % 12) + 12) % 12;
        Scale = scale;
        _intervals = ScaleIntervals(scale);
    }

    public int Degrees => _intervals.Length;

    /// <summary>
    /// Returns the absolute pitch class (0-11) for a given zero-based
    /// scale degree, wrapping across octaves as needed.
    /// </summary>
    public int PitchClassForDegree(int degree)
    {
        var octaveOffset = (int)Math.Floor((double)degree / _intervals.Length);
        var degreeInOctave = degree - (octaveOffset * _intervals.Length);
        return (TonicPitchClass + _intervals[degreeInOctave] + (octaveOffset * 12) + 1200) % 12;
    }

    /// <summary>
    /// Returns a MIDI note number for a scale degree at a given octave
    /// (octave 5 places the tonic near middle C / MIDI 60-71 range).
    /// </summary>
    public int MidiNoteForDegree(int degree, int octave)
    {
        var octaveOffset = (int)Math.Floor((double)degree / _intervals.Length);
        var degreeInOctave = degree - (octaveOffset * _intervals.Length);
        var semitoneFromTonic = _intervals[degreeInOctave] + (octaveOffset * 12);
        return ((octave + 1) * 12) + TonicPitchClass + semitoneFromTonic;
    }

    /// <summary>
    /// True if the given absolute MIDI note's pitch class belongs to this
    /// key's scale (used to decide whether a passing/blue note needs
    /// snapping back onto the scale).
    /// </summary>
    public bool Contains(int midiNote)
    {
        var pc = ((midiNote % 12) + 12) % 12;
        foreach (var interval in _intervals)
        {
            if (((TonicPitchClass + interval) % 12) == pc)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Snaps an arbitrary MIDI note to the nearest pitch class in this key's
    /// scale, preferring downward movement on ties to avoid clashing
    /// against chord tones that sit above it.
    /// </summary>
    public int SnapToScale(int midiNote)
    {
        if (Contains(midiNote))
        {
            return midiNote;
        }

        for (var distance = 1; distance <= 6; distance++)
        {
            if (Contains(midiNote - distance))
            {
                return midiNote - distance;
            }
            if (Contains(midiNote + distance))
            {
                return midiNote + distance;
            }
        }

        return midiNote;
    }

    private static int[] ScaleIntervals(ScaleType scale) => scale switch
    {
        ScaleType.Major => new[] { 0, 2, 4, 5, 7, 9, 11 },
        ScaleType.NaturalMinor => new[] { 0, 2, 3, 5, 7, 8, 10 },
        ScaleType.Dorian => new[] { 0, 2, 3, 5, 7, 9, 10 },
        ScaleType.Mixolydian => new[] { 0, 2, 4, 5, 7, 9, 10 },
        ScaleType.MinorPentatonic => new[] { 0, 3, 5, 7, 10 },
        ScaleType.HarmonicMinor => new[] { 0, 2, 3, 5, 7, 8, 11 },
        _ => throw new ArgumentOutOfRangeException(nameof(scale))
    };

    /// <summary>
    /// The diatonic chord quality built on a given scale degree, for the
    /// seven-note scales. Used to harmonize a roman-numeral progression
    /// into concrete chord qualities for the current key.
    /// </summary>
    public ChordQuality DiatonicQualityForDegree(int degree)
    {
        var degreeInOctave = ((degree % Degrees) + Degrees) % Degrees;

        return Scale switch
        {
            ScaleType.Major => degreeInOctave switch
            {
                0 => ChordQuality.Major,
                1 => ChordQuality.Minor,
                2 => ChordQuality.Minor,
                3 => ChordQuality.Major,
                4 => ChordQuality.Major,
                5 => ChordQuality.Minor,
                6 => ChordQuality.Diminished,
                _ => ChordQuality.Major
            },
            ScaleType.NaturalMinor => degreeInOctave switch
            {
                0 => ChordQuality.Minor,
                1 => ChordQuality.Diminished,
                2 => ChordQuality.Major,
                3 => ChordQuality.Minor,
                4 => ChordQuality.Minor,
                5 => ChordQuality.Major,
                6 => ChordQuality.Major,
                _ => ChordQuality.Minor
            },
            ScaleType.Dorian => degreeInOctave switch
            {
                0 => ChordQuality.Minor,
                1 => ChordQuality.Minor,
                2 => ChordQuality.Major,
                3 => ChordQuality.Major,
                4 => ChordQuality.Minor,
                5 => ChordQuality.Diminished,
                6 => ChordQuality.Major,
                _ => ChordQuality.Minor
            },
            ScaleType.Mixolydian => degreeInOctave switch
            {
                0 => ChordQuality.Major,
                1 => ChordQuality.Minor,
                2 => ChordQuality.Diminished,
                3 => ChordQuality.Major,
                4 => ChordQuality.Minor,
                5 => ChordQuality.Minor,
                6 => ChordQuality.Major,
                _ => ChordQuality.Major
            },
            ScaleType.HarmonicMinor => degreeInOctave switch
            {
                0 => ChordQuality.Minor,
                1 => ChordQuality.Diminished,
                2 => ChordQuality.Augmented,
                3 => ChordQuality.Minor,
                4 => ChordQuality.Major,
                5 => ChordQuality.Major,
                6 => ChordQuality.Diminished,
                _ => ChordQuality.Minor
            },
            _ => ChordQuality.Major
        };
    }

    /// <summary>
    /// Semitone intervals (from the chord root) that make up a given
    /// chord quality. Triads return 3 tones, sevenths return 4.
    /// </summary>
    public static int[] ChordToneIntervals(ChordQuality quality) => quality switch
    {
        ChordQuality.Major => new[] { 0, 4, 7 },
        ChordQuality.Minor => new[] { 0, 3, 7 },
        ChordQuality.Diminished => new[] { 0, 3, 6 },
        ChordQuality.Augmented => new[] { 0, 4, 8 },
        ChordQuality.Dominant7 => new[] { 0, 4, 7, 10 },
        ChordQuality.Major7 => new[] { 0, 4, 7, 11 },
        ChordQuality.Minor7 => new[] { 0, 3, 7, 10 },
        ChordQuality.HalfDiminished7 => new[] { 0, 3, 6, 10 },
        _ => new[] { 0, 4, 7 }
    };
}

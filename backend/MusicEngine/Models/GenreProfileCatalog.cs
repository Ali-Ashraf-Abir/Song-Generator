using backend.MusicEngine.Theory;

namespace backend.MusicEngine.Models;

/// <summary>
/// Builds and caches the immutable <see cref="GenreProfile"/> for each
/// <see cref="MusicGenre"/>. Profiles are built once (static, lazy) and
/// shared across all requests — they contain no per-song state.
/// </summary>
public static class GenreProfileCatalog
{
    private static readonly Dictionary<MusicGenre, GenreProfile> Profiles = BuildAll();

    public static GenreProfile Get(MusicGenre genre) => Profiles[genre];

    /// <summary>Deterministically maps an arbitrary integer onto a genre, used to pick a song's genre from its seed.</summary>
    public static MusicGenre PickGenre(int songSeed)
    {
        var values = Enum.GetValues<MusicGenre>();
        var rng = new Random(HashCode.Combine(songSeed, "genre-pick"));
        return values[rng.Next(values.Length)];
    }

    private static Dictionary<MusicGenre, GenreProfile> BuildAll()
    {
        return new Dictionary<MusicGenre, GenreProfile>
        {
            [MusicGenre.Rock] = BuildRock(),
            [MusicGenre.Pop] = BuildPop(),
            [MusicGenre.Electronic] = BuildElectronic(),
            [MusicGenre.Jazz] = BuildJazz()
        };
    }

    private static GenreProfile BuildRock()
    {
        return new GenreProfile
        {
            Genre = MusicGenre.Rock,
            TempoRange = (100, 150),
            PossibleScales = new[] { ScaleType.Major, ScaleType.NaturalMinor, ScaleType.Mixolydian },
            // Roman-numeral scale degrees (0-indexed): I-V-vi-IV, vi-IV-I-V, I-IV-V-IV, etc.
            ProgressionPool = new[]
            {
                new[] { 0, 4, 5, 3 },
                new[] { 5, 3, 0, 4 },
                new[] { 0, 3, 4, 3 },
                new[] { 0, 5, 3, 4 }
            },
            Instruments = new InstrumentSet
            {
                MelodyProgram = 29, // Overdriven Guitar
                ChordProgram = 30,  // Distortion Guitar
                BassProgram = 33    // Electric Bass (finger)
            },
            DrumPatternPool = new[] { RockDriving(), RockHalfTime() },
            SwingAmount = 0.0,
            SeventhChordBias = 0.10,
            SyncopationBias = 0.25,
            MelodyOctave = 5,
            BassOctave = 3
        };
    }

    private static GenreProfile BuildPop()
    {
        return new GenreProfile
        {
            Genre = MusicGenre.Pop,
            TempoRange = (95, 128),
            PossibleScales = new[] { ScaleType.Major, ScaleType.NaturalMinor },
            ProgressionPool = new[]
            {
                new[] { 0, 4, 5, 3 },
                new[] { 5, 3, 0, 4 },
                new[] { 0, 3, 5, 4 },
                new[] { 5, 4, 0, 4 }
            },
            Instruments = new InstrumentSet
            {
                MelodyProgram = 80, // Lead 1 (square) - bright synth lead
                ChordProgram = 4,   // Electric Piano 1
                BassProgram = 38    // Synth Bass 1
            },
            DrumPatternPool = new[] { PopFour(), PopSyncopated() },
            SwingAmount = 0.0,
            SeventhChordBias = 0.05,
            SyncopationBias = 0.35,
            MelodyOctave = 5,
            BassOctave = 3
        };
    }

    private static GenreProfile BuildElectronic()
    {
        return new GenreProfile
        {
            Genre = MusicGenre.Electronic,
            TempoRange = (118, 132),
            PossibleScales = new[] { ScaleType.NaturalMinor, ScaleType.MinorPentatonic, ScaleType.Dorian },
            ProgressionPool = new[]
            {
                new[] { 0, 0, 5, 3 },
                new[] { 0, 5, 3, 4 },
                new[] { 5, 3, 0, 0 },
                new[] { 0, 3, 0, 4 }
            },
            Instruments = new InstrumentSet
            {
                MelodyProgram = 81, // Lead 2 (sawtooth)
                ChordProgram = 89,  // Pad 2 (warm)
                BassProgram = 39    // Synth Bass 2
            },
            DrumPatternPool = new[] { ElectronicFourOnFloor(), ElectronicBreakbeat() },
            SwingAmount = 0.0,
            SeventhChordBias = 0.0,
            SyncopationBias = 0.45,
            MelodyOctave = 5,
            BassOctave = 2
        };
    }

    private static GenreProfile BuildJazz()
    {
        return new GenreProfile
        {
            Genre = MusicGenre.Jazz,
            TempoRange = (80, 138),
            PossibleScales = new[] { ScaleType.Dorian, ScaleType.Mixolydian, ScaleType.Major, ScaleType.HarmonicMinor },
            // ii-V-I-vi and similar jazz turnarounds.
            ProgressionPool = new[]
            {
                new[] { 1, 4, 0, 5 },
                new[] { 0, 5, 1, 4 },
                new[] { 1, 4, 0, 0 },
                new[] { 5, 1, 4, 0 }
            },
            Instruments = new InstrumentSet
            {
                MelodyProgram = 65, // Alto Sax
                ChordProgram = 0,   // Acoustic Grand Piano
                BassProgram = 32    // Acoustic Bass
            },
            DrumPatternPool = new[] { JazzSwing() },
            SwingAmount = 0.62,
            SeventhChordBias = 0.85,
            SyncopationBias = 0.5,
            MelodyOctave = 5,
            BassOctave = 3
        };
    }

    // ---- Drum pattern templates -------------------------------------------------

    private static DrumPatternTemplate RockDriving() => new()
    {
        Name = "RockDriving",
        Voices = new[]
        {
            new DrumVoiceTemplate { GmKey = GmDrums.BassDrum1, StepProbabilities = Steps(1,0,0,0, 0,0,1,0, 1,0,0,0, 0,0,1,0) },
            new DrumVoiceTemplate { GmKey = GmDrums.AcousticSnare, StepProbabilities = Steps(0,0,0,0, 1,0,0,0, 0,0,0,0, 1,0,0,0) },
            new DrumVoiceTemplate { GmKey = GmDrums.ClosedHiHat, StepProbabilities = Steps(1,1,1,1, 1,1,1,1, 1,1,1,1, 1,1,1,1) },
            new DrumVoiceTemplate { GmKey = GmDrums.CrashCymbal1, StepProbabilities = Steps(0.3,0,0,0, 0,0,0,0, 0,0,0,0, 0,0,0,0) }
        }
    };

    private static DrumPatternTemplate RockHalfTime() => new()
    {
        Name = "RockHalfTime",
        Voices = new[]
        {
            new DrumVoiceTemplate { GmKey = GmDrums.BassDrum1, StepProbabilities = Steps(1,0,0,0, 0,0,0,0, 0,0,1,0, 0,0,0,0) },
            new DrumVoiceTemplate { GmKey = GmDrums.AcousticSnare, StepProbabilities = Steps(0,0,0,0, 0,0,0,0, 1,0,0,0, 0,0,0,0) },
            new DrumVoiceTemplate { GmKey = GmDrums.ClosedHiHat, StepProbabilities = Steps(1,0,1,0, 1,0,1,0, 1,0,1,0, 1,0,1,0) }
        }
    };

    private static DrumPatternTemplate PopFour() => new()
    {
        Name = "PopFour",
        Voices = new[]
        {
            new DrumVoiceTemplate { GmKey = GmDrums.BassDrum1, StepProbabilities = Steps(1,0,0,0, 0,0,0,0.4, 1,0,0,0, 0,0,0,0) },
            new DrumVoiceTemplate { GmKey = GmDrums.ElectricSnare, StepProbabilities = Steps(0,0,0,0, 1,0,0,0, 0,0,0,0, 1,0,0,0) },
            new DrumVoiceTemplate { GmKey = GmDrums.ClosedHiHat, StepProbabilities = Steps(1,0,1,0, 1,0,1,0, 1,0,1,0, 1,0,1,0) }
        }
    };

    private static DrumPatternTemplate PopSyncopated() => new()
    {
        Name = "PopSyncopated",
        Voices = new[]
        {
            new DrumVoiceTemplate { GmKey = GmDrums.BassDrum1, StepProbabilities = Steps(1,0,0,0.6, 0,0,1,0, 0,0,0.5,0, 1,0,0,0) },
            new DrumVoiceTemplate { GmKey = GmDrums.ElectricSnare, StepProbabilities = Steps(0,0,0,0, 1,0,0,0, 0,0,0,0, 1,0,0,0.3) },
            new DrumVoiceTemplate { GmKey = GmDrums.ClosedHiHat, StepProbabilities = Steps(1,1,1,1, 1,1,1,1, 1,1,1,1, 1,1,1,1) }
        }
    };

    private static DrumPatternTemplate ElectronicFourOnFloor() => new()
    {
        Name = "ElectronicFourOnFloor",
        Voices = new[]
        {
            new DrumVoiceTemplate { GmKey = GmDrums.BassDrum1, StepProbabilities = Steps(1,0,0,0, 1,0,0,0, 1,0,0,0, 1,0,0,0) },
            new DrumVoiceTemplate { GmKey = GmDrums.ElectricSnare, StepProbabilities = Steps(0,0,0,0, 0,0,0,0, 0,0,0,0, 1,0,0,0) },
            new DrumVoiceTemplate { GmKey = GmDrums.OpenHiHat, StepProbabilities = Steps(0,0,1,0, 0,0,1,0, 0,0,1,0, 0,0,1,0) },
            new DrumVoiceTemplate { GmKey = GmDrums.ClosedHiHat, StepProbabilities = Steps(0,1,0,1, 0,1,0,1, 0,1,0,1, 0,1,0,1) }
        }
    };

    private static DrumPatternTemplate ElectronicBreakbeat() => new()
    {
        Name = "ElectronicBreakbeat",
        Voices = new[]
        {
            new DrumVoiceTemplate { GmKey = GmDrums.BassDrum1, StepProbabilities = Steps(1,0,0,0.5, 0,0,1,0, 0,0.4,0,0, 1,0,0,0) },
            new DrumVoiceTemplate { GmKey = GmDrums.ElectricSnare, StepProbabilities = Steps(0,0,0,0, 1,0,0,0.3, 0,0,0,0.4, 1,0,0,0) },
            new DrumVoiceTemplate { GmKey = GmDrums.ClosedHiHat, StepProbabilities = Steps(1,1,1,1, 1,1,1,1, 1,1,1,1, 1,1,1,1) }
        }
    };

    private static DrumPatternTemplate JazzSwing() => new()
    {
        Name = "JazzSwing",
        Voices = new[]
        {
            // Ride pattern carries the swing feel; kick/snare are sparse comping accents.
            new DrumVoiceTemplate { GmKey = GmDrums.RideCymbal1, StepProbabilities = Steps(1,0,0.7,0, 1,0,0.7,0, 1,0,0.7,0, 1,0,0.7,0) },
            new DrumVoiceTemplate { GmKey = GmDrums.BassDrum1, StepProbabilities = Steps(0.4,0,0,0, 0,0,0.3,0, 0.4,0,0,0, 0,0,0.3,0) },
            new DrumVoiceTemplate { GmKey = GmDrums.AcousticSnare, StepProbabilities = Steps(0,0,0,0.3, 0,0,0,0, 0,0,0,0.3, 0,0,0,0) }
        }
    };

    private static double[] Steps(
        double s0, double s1, double s2, double s3,
        double s4, double s5, double s6, double s7,
        double s8, double s9, double s10, double s11,
        double s12, double s13, double s14, double s15)
        => new[] { s0, s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s11, s12, s13, s14, s15 };
}

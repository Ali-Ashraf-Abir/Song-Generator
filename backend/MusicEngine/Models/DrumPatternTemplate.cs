namespace backend.MusicEngine.Models;

/// <summary>
/// General MIDI percussion key numbers (channel 10) used by the drum
/// generator. Only the subset the engine actually plays.
/// </summary>
public static class GmDrums
{
    public const int AcousticBassDrum = 35;
    public const int BassDrum1 = 36;
    public const int AcousticSnare = 38;
    public const int ElectricSnare = 40;
    public const int ClosedHiHat = 42;
    public const int OpenHiHat = 46;
    public const int CrashCymbal1 = 49;
    public const int RideCymbal1 = 51;
}

/// <summary>
/// A 16-step-per-bar (16th note resolution) probability template for one
/// drum voice. A value of 1.0 means "always hit", 0.0 means "never";
/// values in between are resolved deterministically against the seeded
/// RNG by the drum generator, so the same seed always yields the same
/// pattern even though the template itself encodes a range of feels.
/// </summary>
public sealed class DrumVoiceTemplate
{
    public required int GmKey { get; init; }
    public required double[] StepProbabilities { get; init; } // length 16
}

/// <summary>
/// A full one-bar drum pattern template (kick/snare/hihat/etc.) for a
/// genre. Genres carry several of these in their pool so the drum
/// generator can pick (deterministically, per seed) between a couple of
/// stylistically valid grooves rather than always using the same one.
/// </summary>
public sealed class DrumPatternTemplate
{
    public required string Name { get; init; }
    public required IReadOnlyList<DrumVoiceTemplate> Voices { get; init; }
}

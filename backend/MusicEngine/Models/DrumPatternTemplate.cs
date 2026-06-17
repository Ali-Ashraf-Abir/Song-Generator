namespace backend.MusicEngine.Models;

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


public sealed class DrumVoiceTemplate
{
    public required int GmKey { get; init; }
    public required double[] StepProbabilities { get; init; } // length 16
}

public sealed class DrumPatternTemplate
{
    public required string Name { get; init; }
    public required IReadOnlyList<DrumVoiceTemplate> Voices { get; init; }
}

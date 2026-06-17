using backend.MusicEngine.Models;

namespace backend.MusicEngine.Generation;

public static class DrumPatternGenerator
{
    private const int StepsPerBar = 16;

    public static IReadOnlyList<DrumHitEvent> Generate(
        GenreProfile profile,
        int songSeed,
        int totalBars,
        int beatsPerBar)
    {
        var templateRng = new Random(HashCode.Combine(songSeed, "drum-template-pick"));
        var template = profile.DrumPatternPool[templateRng.Next(profile.DrumPatternPool.Count)];

        var hits = new List<DrumHitEvent>();
        var beatsPerStep = beatsPerBar / (double)StepsPerBar * 4.0; // 16 steps spans 4 beats (one 4/4 bar)

        foreach (var voice in template.Voices)
        {
            for (var bar = 0; bar < totalBars; bar++)
            {
                var voiceRng = new Random(HashCode.Combine(songSeed, "drum-voice", voice.GmKey, bar));
                var velocityRng = new Random(HashCode.Combine(songSeed, "drum-velocity", voice.GmKey, bar));

                for (var step = 0; step < StepsPerBar; step++)
                {
                    var probability = voice.StepProbabilities[step];
                    if (probability <= 0.0)
                    {
                        continue;
                    }

                    if (voiceRng.NextDouble() < probability)
                    {
                        var startBeat = (bar * beatsPerBar) + (step * beatsPerStep);
                        var accentBoost = (step % 4 == 0) ? 12 : 0;
                        var velocity = Math.Clamp(75 + accentBoost + velocityRng.Next(-10, 11), 1, 127);

                        hits.Add(new DrumHitEvent
                        {
                            GmKey = voice.GmKey,
                            StartBeat = startBeat,
                            Velocity = velocity
                        });
                    }
                }
            }
        }

        return hits;
    }
}

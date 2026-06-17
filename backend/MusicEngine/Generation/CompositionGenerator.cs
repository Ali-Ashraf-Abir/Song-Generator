using backend.MusicEngine.Models;

namespace backend.MusicEngine.Generation;

public sealed class CompositionGenerator : ICompositionGenerator
{
    private const int BeatsPerBar = 4;

    public CompositionResult Generate(int songSeed, int targetDurationSeconds)
    {
        var genre = GenreProfileCatalog.PickGenre(songSeed);
        var profile = GenreProfileCatalog.Get(genre);

        var key = KeyTempoSelector.SelectKey(profile, songSeed);
        var tempo = KeyTempoSelector.SelectTempo(profile, songSeed);

        var totalBars = CalculateBarCount(tempo, targetDurationSeconds);

        var chordProgression = ChordProgressionGenerator.Generate(profile, key, songSeed, totalBars);

        var melodyNotes = MelodyGenerator.Generate(profile, key, chordProgression, songSeed, BeatsPerBar);
        var chordNotes = ChordPadGenerator.Generate(profile, key, chordProgression, songSeed, BeatsPerBar);
        var bassNotes = BasslineGenerator.Generate(profile, key, chordProgression, songSeed, BeatsPerBar);
        var drumHits = DrumPatternGenerator.Generate(profile, songSeed, totalBars, BeatsPerBar);

        return new CompositionResult
        {
            Genre = genre,
            Profile = profile,
            Key = key,
            Tempo = tempo,
            TotalBars = totalBars,
            BeatsPerBar = BeatsPerBar,
            ChordProgression = chordProgression,
            MelodyNotes = melodyNotes,
            ChordNotes = chordNotes,
            BassNotes = bassNotes,
            DrumHits = drumHits
        };
    }

    private static int CalculateBarCount(int tempo, int targetDurationSeconds)
    {
        var secondsPerBeat = 60.0 / tempo;
        var secondsPerBar = secondsPerBeat * BeatsPerBar;

        var rawBars = targetDurationSeconds / secondsPerBar;
        var phraseBars = (int)Math.Ceiling(rawBars / 4.0) * 4;

        return Math.Max(phraseBars, 4);
    }
}

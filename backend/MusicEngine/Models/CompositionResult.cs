using backend.MusicEngine.Theory;

namespace backend.MusicEngine.Models;


public sealed class NoteEvent
{
    public required int MidiNote { get; init; }
    public required double StartBeat { get; init; }
    public required double DurationBeats { get; init; }
    public required int Velocity { get; init; } // 1-127
}


public sealed class DrumHitEvent
{
    public required int GmKey { get; init; }
    public required double StartBeat { get; init; }
    public required int Velocity { get; init; }
}


public sealed class CompositionResult
{
    public required MusicGenre Genre { get; init; }
    public required GenreProfile Profile { get; init; }
    public required MusicKey Key { get; init; }
    public required int Tempo { get; init; }
    public required int TotalBars { get; init; }
    public required int BeatsPerBar { get; init; }
    public required IReadOnlyList<Chord> ChordProgression { get; init; } // one chord per bar, repeating
    public required IReadOnlyList<NoteEvent> MelodyNotes { get; init; }
    public required IReadOnlyList<NoteEvent> ChordNotes { get; init; }
    public required IReadOnlyList<NoteEvent> BassNotes { get; init; }
    public required IReadOnlyList<DrumHitEvent> DrumHits { get; init; }
}

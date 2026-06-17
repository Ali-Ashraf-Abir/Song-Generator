using backend.MusicEngine.Theory;

namespace backend.MusicEngine.Models;

/// <summary>
/// One played note: pitch, start time, duration, and velocity, all in
/// musical units (beats) rather than ticks or seconds — conversion to
/// ticks happens in the MIDI builder, conversion to seconds happens in
/// nothing (we render straight from MIDI), keeping this struct reusable.
/// </summary>
public sealed class NoteEvent
{
    public required int MidiNote { get; init; }
    public required double StartBeat { get; init; }
    public required double DurationBeats { get; init; }
    public required int Velocity { get; init; } // 1-127
}

/// <summary>
/// One drum hit: a GM percussion key number plus timing/velocity. Always
/// rendered to MIDI channel 10.
/// </summary>
public sealed class DrumHitEvent
{
    public required int GmKey { get; init; }
    public required double StartBeat { get; init; }
    public required int Velocity { get; init; }
}

/// <summary>
/// The complete abstract composition for one song: everything the MIDI
/// builder needs and nothing it has to decide for itself. Every field is
/// fully deterministic given the originating seed.
/// </summary>
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

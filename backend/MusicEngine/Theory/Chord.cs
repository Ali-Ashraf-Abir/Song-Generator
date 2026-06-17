namespace backend.MusicEngine.Theory;

/// <summary>
/// A single resolved chord: a root pitch class, a quality, and the
/// concrete chord-tone pitch classes derived from them. Octave placement
/// happens later (in the bass/melody/pad generators), so this stays
/// octave-agnostic and easy to reason about.
/// </summary>
public sealed class Chord
{
    public int RootPitchClass { get; }
    public ChordQuality Quality { get; }
    public int ScaleDegree { get; }

    /// <summary>Pitch classes (0-11) making up the chord, root first.</summary>
    public IReadOnlyList<int> TonePitchClasses { get; }

    public Chord(int rootPitchClass, ChordQuality quality, int scaleDegree)
    {
        RootPitchClass = ((rootPitchClass % 12) + 12) % 12;
        Quality = quality;
        ScaleDegree = scaleDegree;

        TonePitchClasses = MusicKey
            .ChordToneIntervals(quality)
            .Select(interval => (RootPitchClass + interval) % 12)
            .ToList();
    }

    /// <summary>
    /// Returns the MIDI note for the chord tone at <paramref name="toneIndex"/>
    /// (0 = root, 1 = third, 2 = fifth, 3 = seventh if present), placed in
    /// the octave nearest to <paramref name="anchorMidiNote"/>.
    /// </summary>
    public int ToneNearOctave(int toneIndex, int anchorMidiNote)
    {
        var pitchClass = TonePitchClasses[((toneIndex % TonePitchClasses.Count) + TonePitchClasses.Count) % TonePitchClasses.Count];

        var candidate = (anchorMidiNote / 12 * 12) + pitchClass;

        // Pick whichever octave puts the candidate closest to the anchor.
        var best = candidate;
        var bestDistance = Math.Abs(candidate - anchorMidiNote);

        foreach (var delta in new[] { -24, -12, 12, 24 })
        {
            var alt = candidate + delta;
            var dist = Math.Abs(alt - anchorMidiNote);
            if (dist < bestDistance)
            {
                best = alt;
                bestDistance = dist;
            }
        }

        return best;
    }

    /// <summary>True if the pitch class of <paramref name="midiNote"/> is one of this chord's tones.</summary>
    public bool IsChordTone(int midiNote)
    {
        var pc = ((midiNote % 12) + 12) % 12;
        return TonePitchClasses.Contains(pc);
    }
}

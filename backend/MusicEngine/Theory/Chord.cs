namespace backend.MusicEngine.Theory;


public sealed class Chord
{
    public int RootPitchClass { get; }
    public ChordQuality Quality { get; }
    public int ScaleDegree { get; }

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


    public bool IsChordTone(int midiNote)
    {
        var pc = ((midiNote % 12) + 12) % 12;
        return TonePitchClasses.Contains(pc);
    }
}

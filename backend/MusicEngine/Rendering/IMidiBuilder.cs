using backend.MusicEngine.Models;
using Melanchall.DryWetMidi.Core;

namespace backend.MusicEngine.Rendering;

public interface IMidiBuilder
{
    /// <summary>Builds a complete multi-track MIDI file from a composition.</summary>
    MidiFile Build(CompositionResult composition);
}

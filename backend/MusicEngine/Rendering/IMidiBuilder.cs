using backend.MusicEngine.Models;
using Melanchall.DryWetMidi.Core;

namespace backend.MusicEngine.Rendering;

public interface IMidiBuilder
{
    
    MidiFile Build(CompositionResult composition);
}

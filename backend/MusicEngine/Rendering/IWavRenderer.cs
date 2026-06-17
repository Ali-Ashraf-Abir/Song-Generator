namespace backend.MusicEngine.Rendering;

public interface IWavRenderer
{

    byte[] Render(Melanchall.DryWetMidi.Core.MidiFile midiFile);
}

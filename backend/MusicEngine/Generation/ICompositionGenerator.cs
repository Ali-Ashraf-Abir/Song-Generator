using backend.MusicEngine.Models;

namespace backend.MusicEngine.Generation;

public interface ICompositionGenerator
{
    CompositionResult Generate(int songSeed, int targetDurationSeconds);
}

using backend.Dtos;

namespace backend.Services;

public interface ISongAudioService
{
    /// <summary>
    /// Returns WAV bytes for the requested song preview, using the cache
    /// when available and rendering (then caching) on a miss. The
    /// returned audio is fully deterministic for a given request.
    /// </summary>
    Task<byte[]> GetPreviewWavAsync(AudioPreviewRequest request, CancellationToken cancellationToken = default);
}

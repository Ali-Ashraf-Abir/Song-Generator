namespace backend.MusicEngine.Caching;

public interface IAudioCache
{
  
    Task<byte[]?> TryGetAsync(string cacheKey, CancellationToken cancellationToken = default);


    Task SetAsync(string cacheKey, byte[] wavBytes, CancellationToken cancellationToken = default);
}

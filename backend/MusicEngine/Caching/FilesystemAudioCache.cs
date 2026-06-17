using System.Security.Cryptography;
using System.Text;

namespace backend.MusicEngine.Caching;

public sealed class FilesystemAudioCache : IAudioCache
{

    private const string CacheVersion = "v1";

    private readonly string _cacheDirectory;

    public FilesystemAudioCache(string cacheDirectory)
    {
        _cacheDirectory = cacheDirectory;
        Directory.CreateDirectory(_cacheDirectory);
    }

    public async Task<byte[]?> TryGetAsync(string cacheKey, CancellationToken cancellationToken = default)
    {
        var path = PathFor(cacheKey);

        if (!File.Exists(path))
        {
            return null;
        }

        try
        {
            return await File.ReadAllBytesAsync(path, cancellationToken);
        }
        catch (IOException)
        {
            // Another request may be writing this exact entry concurrently;
            // treat as a cache miss rather than failing the request.
            return null;
        }
    }

    public async Task SetAsync(string cacheKey, byte[] wavBytes, CancellationToken cancellationToken = default)
    {
        var path = PathFor(cacheKey);

        // Write to a unique temp file then rename, so concurrent requests
        // racing to populate the same cache entry never read a partially
        // written file, and the final rename is atomic on the same volume.
        var tempPath = $"{path}.{Guid.NewGuid():N}.tmp";

        await File.WriteAllBytesAsync(tempPath, wavBytes, cancellationToken);

        try
        {
            File.Move(tempPath, path, overwrite: true);
        }
        catch (IOException)
        {
            // Another request already populated this entry; our temp file
            // is redundant, so just discard it.
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }
    }

    private string PathFor(string cacheKey)
    {
        var safeKey = HashKey(cacheKey);
        return Path.Combine(_cacheDirectory, $"{CacheVersion}_{safeKey}.wav");
    }

    private static string HashKey(string cacheKey)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(cacheKey));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}

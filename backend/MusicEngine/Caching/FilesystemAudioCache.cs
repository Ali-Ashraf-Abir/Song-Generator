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

            return null;
        }
    }

    public async Task SetAsync(string cacheKey, byte[] wavBytes, CancellationToken cancellationToken = default)
    {
        var path = PathFor(cacheKey);


        var tempPath = $"{path}.{Guid.NewGuid():N}.tmp";

        await File.WriteAllBytesAsync(tempPath, wavBytes, cancellationToken);

        try
        {
            File.Move(tempPath, path, overwrite: true);
        }
        catch (IOException)
        {

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

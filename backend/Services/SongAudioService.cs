using backend.Dtos;
using backend.MusicEngine.Caching;
using backend.MusicEngine.Generation;
using backend.MusicEngine.Rendering;

namespace backend.Services;

public sealed class SongAudioService : ISongAudioService
{
    private readonly ICompositionGenerator _compositionGenerator;
    private readonly IMidiBuilder _midiBuilder;
    private readonly IWavRenderer _wavRenderer;
    private readonly IAudioCache _audioCache;

    public SongAudioService(
        ICompositionGenerator compositionGenerator,
        IMidiBuilder midiBuilder,
        IWavRenderer wavRenderer,
        IAudioCache audioCache)
    {
        _compositionGenerator = compositionGenerator;
        _midiBuilder = midiBuilder;
        _wavRenderer = wavRenderer;
        _audioCache = audioCache;
    }

    public async Task<byte[]> GetPreviewWavAsync(
        AudioPreviewRequest request,
        CancellationToken cancellationToken = default)
    {
        var songSeed = DeriveSongSeed(request);
        var cacheKey = BuildCacheKey(songSeed, request.DurationSeconds);

        var cached = await _audioCache.TryGetAsync(cacheKey, cancellationToken);
        if (cached is not null)
        {
            return cached;
        }

        var composition = _compositionGenerator.Generate(songSeed, request.DurationSeconds);
        var midiFile = _midiBuilder.Build(composition);
        var wavBytes = _wavRenderer.Render(midiFile);

        await _audioCache.SetAsync(cacheKey, wavBytes, cancellationToken);

        return wavBytes;
    }

    private static int DeriveSongSeed(AudioPreviewRequest request)
    {
        return HashCode.Combine(request.Seed, request.Index);
    }

    private static string BuildCacheKey(int songSeed, int durationSeconds)
        => $"song_{songSeed}_dur{durationSeconds}";
}

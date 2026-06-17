using backend.MusicEngine.Caching;
using backend.MusicEngine.Generation;
using backend.MusicEngine.Rendering;
using backend.Services;

namespace backend.MusicEngine;

public static class MusicEngineServiceCollectionExtensions
{
    public static IServiceCollection AddMusicEngine(
        this IServiceCollection services,
        MusicEngineOptions options)
    {
        services.AddSingleton<ICompositionGenerator, CompositionGenerator>();
        services.AddSingleton<IMidiBuilder, MidiBuilder>();

        services.AddSingleton<IWavRenderer>(_ => new WavRenderer(options.SoundFontPath));

        services.AddSingleton<IAudioCache>(_ => new FilesystemAudioCache(options.CacheDirectory));

        services.AddSingleton<ISongAudioService, SongAudioService>();

        return services;
    }
}

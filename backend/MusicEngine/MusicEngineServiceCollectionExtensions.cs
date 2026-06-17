using backend.MusicEngine.Caching;
using backend.MusicEngine.Generation;
using backend.MusicEngine.Rendering;
using backend.Services;

namespace backend.MusicEngine;

public static class MusicEngineServiceCollectionExtensions
{
    /// <summary>
    /// Registers the procedural music engine: composition generation,
    /// MIDI building, WAV rendering, filesystem caching, and the
    /// top-level <see cref="ISongAudioService"/> facade used by
    /// AudioController.
    /// </summary>
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

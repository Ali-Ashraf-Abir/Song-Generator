namespace backend.MusicEngine;

/// <summary>
/// Bind this to a "MusicEngine" section in appsettings.json (or set via
/// environment variables) to configure where the SoundFont and audio
/// cache live.
/// </summary>
public sealed class MusicEngineOptions
{
    /// <summary>
    /// Absolute or app-relative path to a General MIDI-compatible .sf2
    /// SoundFont file. MeltySynth needs this to render any audio at all.
    /// A reasonable freely-licensed default is GeneralUser GS; place the
    /// .sf2 file somewhere on disk and point this at it.
    /// </summary>
    public string SoundFontPath { get; set; } = "MusicEngine/SoundFonts/GeneralUser-GS.sf2";

    /// <summary>Directory where rendered WAV previews are cached.</summary>
    public string CacheDirectory { get; set; } = "MusicEngine/Cache";
}

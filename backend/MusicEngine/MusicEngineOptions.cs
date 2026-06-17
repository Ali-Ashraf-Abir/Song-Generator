namespace backend.MusicEngine;

public sealed class MusicEngineOptions
{
    public string SoundFontPath { get; set; } = "MusicEngine/SoundFonts/GeneralUser-GS.sf2";

   
    public string CacheDirectory { get; set; } = "MusicEngine/Cache";
}

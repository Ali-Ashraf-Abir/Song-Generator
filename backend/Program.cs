using backend.Providers;
using backend.Services;
using backend.MusicEngine;
var builder = WebApplication.CreateBuilder(args);
var allowedOrigins = builder.Configuration["CORS_ALLOWED_ORIGINS"]?
    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
var musicEngineOptions = new MusicEngineOptions();
musicEngineOptions.SoundFontPath =
    Path.Combine(
        builder.Environment.ContentRootPath,
        musicEngineOptions.SoundFontPath);

musicEngineOptions.CacheDirectory =
    Path.Combine(
        builder.Environment.ContentRootPath,
        musicEngineOptions.CacheDirectory);
builder.Configuration.GetSection("MusicEngine").Bind(musicEngineOptions);
builder.Services.AddMusicEngine(musicEngineOptions);
Console.WriteLine(
    Path.GetFullPath("SoundFonts/GeneralUser_GS.sf2"));

Console.WriteLine(
    File.Exists("SoundFonts/GeneralUser_GS.sf2"));
Console.WriteLine(musicEngineOptions.SoundFontPath);
builder.Services.AddControllers();
builder.Services.AddSingleton<ILocalizationProvider, LocalizationProvider>();

builder.Services.AddScoped<ISongGenerationService, SongGenerationService>();
var app = builder.Build();
app.MapGet("/", () => "Server is online");
app.UseCors("AllowFrontend");
app.MapControllers();
app.Run();

using backend.Providers;
using backend.Services;

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


builder.Services.AddControllers();
builder.Services.AddSingleton<ILocalizationProvider, LocalizationProvider>();

builder.Services.AddScoped<ISongGenerationService, SongGenerationService>();
var app = builder.Build();
app.MapGet("/", () => "Server is online");
app.UseCors("AllowFrontend");
app.MapControllers();
app.Run();

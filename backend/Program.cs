using backend.Providers;
using backend.Services;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();
builder.Services.AddSingleton<ILocalizationProvider, LocalizationProvider>();

builder.Services.AddScoped<ISongGenerationService, SongGenerationService>();
var app = builder.Build();
app.MapGet("/", () => "Server is online");

app.MapControllers();
app.Run();

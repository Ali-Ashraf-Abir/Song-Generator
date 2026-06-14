namespace backend.Providers;

using System.Text.Json;

public sealed class LocalizationProvider : ILocalizationProvider
{
    private readonly Dictionary<string, LocaleData> _cache;

    public LocalizationProvider(IWebHostEnvironment env)
    {
        _cache = new Dictionary<string, LocaleData>(
            StringComparer.OrdinalIgnoreCase);

        var path = Path.Combine(
            env.ContentRootPath,
            "Locales");

        var files = Directory.GetFiles(
            path,
            "*.json",
            SearchOption.AllDirectories);

        foreach (var file in files)
        {
            var key =
                Path.GetFileNameWithoutExtension(file);

            var json =
                File.ReadAllText(file);

            var data = JsonSerializer.Deserialize<LocaleData>(
     json,
     new JsonSerializerOptions
     {
         PropertyNameCaseInsensitive = true
     });

            if (data != null)
            {
                _cache[key] = data;

                Console.WriteLine(
                    $"Loaded locale: {key}");
            }
        }
    }

    public LocaleData Get(string locale)
    {
        if (_cache.TryGetValue(locale, out var data))
        {
            return data;
        }

        throw new InvalidOperationException(
            $"Locale '{locale}' not found. Available locales: {string.Join(", ", _cache.Keys)}");
    }
}
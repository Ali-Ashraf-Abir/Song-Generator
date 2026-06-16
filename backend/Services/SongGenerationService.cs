namespace backend.Services;

using backend.Dtos;

public class SongGenerationService : ISongGenerationService
{
    private readonly ILocalizationProvider _localizationProvider;

    public SongGenerationService(ILocalizationProvider localizationProvider)
    {
        _localizationProvider = localizationProvider;
    }

    public Task<IReadOnlyList<SongDto>> GenerateAsync(
           SongGenerationRequest request)
    {
        var locale = _localizationProvider.Get(request.Locale);

        var songs = new List<SongDto>();

        var startIndex =
            (request.Page - 1) * request.PageSize;

        for (var i = 0; i < request.PageSize; i++)
        {
            var globalIndex = startIndex + i + 1;

            var songSeed =
                HashCode.Combine(
                    request.Seed,
                    globalIndex);

            songs.Add(
             GenerateSong(
                 locale,
                 request.Locale,
                 songSeed,
                 globalIndex,
                 request.AvgLikes));
        }

        return Task.FromResult<IReadOnlyList<SongDto>>(songs);
    }

    private SongDto GenerateSong(
        LocaleData locale,
        string localeCode,
        int songSeed,
        int index,
        double avgLikes)
    {
        var titleRandom =
            new Random(HashCode.Combine(songSeed, "title"));

        var artistRandom =
            new Random(HashCode.Combine(songSeed, "artist"));

        var albumRandom =
            new Random(HashCode.Combine(songSeed, "album"));

        var genreRandom =
            new Random(HashCode.Combine(songSeed, "genre"));

        var likesRandom =
            new Random(HashCode.Combine(songSeed, "likes"));

        var titleLength = titleRandom.NextDouble() switch
        {
            < 0.15 => 1,
            < 0.60 => 2,
            _ => 3
        };
        var title = titleLength switch
        {
            1 => Pick(locale.SongNouns, titleRandom),
            2 => $"{Pick(locale.SongAdjectives, titleRandom)} {Pick(locale.SongNouns, titleRandom)}",
            _ => $"{Pick(locale.SongAdjectives, titleRandom)} {Pick(locale.SongAdjectives, titleRandom)} {Pick(locale.SongNouns, titleRandom)}"
        };

        var artist = GenerateArtist(locale, artistRandom);

        var album = GenerateAlbum(locale, albumRandom);

        var genre = Pick(locale.Genres, genreRandom);
        return new SongDto
        {
            Index = index,
            Title = title,
            Artist = artist,
            Album = album,
            Genre = genre,
            Likes = GenerateLikes(avgLikes, likesRandom),
            CoverUrl = $"/api/covers?seed={songSeed}&index={index}&locale={localeCode}"
        };
    }

    private static string GenerateArtist(
        LocaleData locale,
        Random random)
    {
        if (random.NextDouble() < 0.5)
        {
            return $"{Pick(locale.BandPrefixes, random)} " +
                   $"{Pick(locale.BandNouns, random)}";
        }

        return $"{Pick(locale.FirstNames, random)} " +
               $"{Pick(locale.LastNames, random)}";
    }

    private static string GenerateAlbum(
        LocaleData locale,
        Random random)
    {
        if (random.NextDouble() < 0.25)
            return "Single";

        return $"{Pick(locale.AlbumAdjectives, random)} " +
               $"{Pick(locale.AlbumNouns, random)}";
    }

    private static string Pick(
        IReadOnlyList<string> source,
        Random random)
    {
        return source[random.Next(source.Count)];
    }

    private static int GenerateLikes(
        double average,
        Random random)
    {
        var whole = (int)Math.Floor(average);

        var fraction = average - whole;

        return whole +
               (random.NextDouble() < fraction
                    ? 1
                    : 0);
    }

    public SongDto GenerateFromSeed(
    string locale,
    int songSeed,
    int index)
    {
        var localeData =
            _localizationProvider.Get(locale);

        return GenerateSong(
            localeData,
            locale,
            songSeed,
            index,
            3.7);
    }
}
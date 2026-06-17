using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/covers")]
public class CoverController : ControllerBase
{
    private readonly ISongGenerationService _songGenerationService;

    public CoverController(ISongGenerationService songGenerationService)
    {
        _songGenerationService = songGenerationService;
    }

    [HttpGet]
    public IActionResult GetCover(
        [FromQuery] int seed,
        [FromQuery] int index,
        [FromQuery] string locale = "en-US")
    {
        var song = _songGenerationService.GenerateFromSeed(locale, seed, index);
        var bytes = CoverGenerator.Generate(seed, song.Title, song.Album,song.Artist);
        return File(bytes, "image/png");
    }
}
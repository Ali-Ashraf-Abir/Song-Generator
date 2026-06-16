using backend.Dtos;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SongController : ControllerBase
{
    private readonly ISongGenerationService _songGenerationService;

    public SongController(
        ISongGenerationService songGenerationService)
    {
        _songGenerationService = songGenerationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetSongs(
    [FromQuery] string locale = "en-US",
    [FromQuery] long seed = 1,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20,
    [FromQuery] double avgLikes = 3.7)
    {
        var request = new SongGenerationRequest
        {
            Locale = locale,
            Seed = seed,
            Page = page,
            PageSize = pageSize,
            AvgLikes = avgLikes
        };

        var songs =
            await _songGenerationService.GenerateAsync(request);

        return Ok(songs);
    }
}
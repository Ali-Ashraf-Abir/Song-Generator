using backend.Dtos;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AudioController : ControllerBase
{
    private readonly ISongAudioService _songAudioService;

    public AudioController(ISongAudioService songAudioService)
    {
        _songAudioService = songAudioService;
    }

    [HttpGet("preview")]
    public async Task<IActionResult> GetPreview(
        [FromQuery] long seed = 1,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] int index = 1,
        [FromQuery] int durationSeconds = 25,
        CancellationToken cancellationToken = default)
    {
        if (index < 1)
        {
            return BadRequest("index must be 1 or greater.");
        }

        if (durationSeconds is < 5 or > 60)
        {
            return BadRequest("durationSeconds must be between 5 and 60.");
        }

        var request = new AudioPreviewRequest
        {
            Seed = seed,
            Page = page,
            PageSize = pageSize,
            Index = index,
            DurationSeconds = durationSeconds
        };

        var wavBytes = await _songAudioService.GetPreviewWavAsync(request, cancellationToken);

        return new FileContentResult(wavBytes, "audio/wav")
        {
            FileDownloadName = $"preview_{seed}_{index}.wav"
        };
    }
}

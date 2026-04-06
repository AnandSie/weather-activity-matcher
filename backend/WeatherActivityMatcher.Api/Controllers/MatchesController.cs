using Microsoft.AspNetCore.Mvc;
using WeatherActivityMatcher.Api.Services;

namespace WeatherActivityMatcher.Api.Controllers;

[ApiController]
[Route("api/v1/matches")]
public class MatchesController(MatcherService matcherService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var matches = await matcherService.GetMatchesAsync(ct);
        return Ok(matches);
    }
}

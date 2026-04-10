using Microsoft.AspNetCore.Mvc;
using WeatherActivityMatcher.Api.Services;

namespace WeatherActivityMatcher.Api.Controllers;

[ApiController]
[Route("api/v1/admin")]
public class AdminController(WeatherService weatherService, MatcherService matcherService) : ControllerBase
{
    [HttpPost("refresh-weather")]
    public async Task<IActionResult> RefreshWeather(CancellationToken ct)
    {
        await weatherService.FetchAndStoreAsync(ct);
        return Ok(new { message = "Weather refreshed" });
    }

    [HttpPost("run-matching")]
    public async Task<IActionResult> RunMatching(CancellationToken ct)
    {
        await matcherService.RunMatchingAsync(ct);
        return Ok(new { message = "Matching complete" });
    }
}

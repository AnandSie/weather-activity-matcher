using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeatherActivityMatcher.Api.Data;
using WeatherActivityMatcher.Api.Dtos;

namespace WeatherActivityMatcher.Api.Controllers;

[ApiController]
[Route("api/v1/forecasts")]
public class ForecastsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int hours = 168)
    {
        var cutoff = DateTime.UtcNow;
        var until = cutoff.AddHours(hours);

        var rows = await db.HourlyForecasts
            .Where(f => f.ValidTime >= cutoff && f.ValidTime <= until)
            .OrderBy(f => f.ValidTime)
            .Select(f => new ForecastHourDto(f.ValidTime, f.Temperature2m, f.Windspeed10m, f.Precipitation, f.Cloudcover))
            .ToListAsync();

        return Ok(rows);
    }
}

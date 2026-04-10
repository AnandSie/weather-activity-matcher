using Microsoft.AspNetCore.Mvc;
using WeatherActivityMatcher.Api.Data;
using WeatherActivityMatcher.Api.Dtos;
using WeatherActivityMatcher.Api.Models;

namespace WeatherActivityMatcher.Api.Controllers;

[ApiController]
[Route("api/v1/settings")]
public class SettingsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var settings = await db.UserSettings.FindAsync(1) ?? new UserSettings();
        return Ok(new SettingsDto(settings.Latitude, settings.Longitude, settings.LocationName));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] SettingsDto dto)
    {
        var settings = await db.UserSettings.FindAsync(1);
        if (settings is null)
        {
            settings = new UserSettings { Id = 1 };
            db.UserSettings.Add(settings);
        }
        settings.Latitude = dto.Latitude;
        settings.Longitude = dto.Longitude;
        settings.LocationName = dto.LocationName;
        await db.SaveChangesAsync();
        return Ok(new SettingsDto(settings.Latitude, settings.Longitude, settings.LocationName));
    }
}

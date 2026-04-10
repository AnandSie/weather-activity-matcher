using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeatherActivityMatcher.Api.Data;
using WeatherActivityMatcher.Api.Dtos;
using WeatherActivityMatcher.Api.Models;
using WeatherActivityMatcher.Api.Services;

namespace WeatherActivityMatcher.Api.Controllers;

[ApiController]
[Route("api/v1/activities")]
public class ActivitiesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var activities = await db.Activities
            .Include(a => a.Constraints)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => MapToDto(a))
            .ToListAsync();
        return Ok(activities);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var activity = await db.Activities.Include(a => a.Constraints).FirstOrDefaultAsync(a => a.Id == id);
        if (activity is null) return NotFound();
        return Ok(MapToDto(activity));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ActivityCreateDto dto)
    {
        var activity = new Activity
        {
            Name = dto.Name,
            TemplateSlug = dto.TemplateSlug,
            Enabled = dto.Enabled,
            CreatedAt = DateTime.UtcNow,
            Constraints = MapConstraints(dto.Constraints),
        };

        db.Activities.Add(activity);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = activity.Id }, MapToDto(activity));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ActivityUpdateDto dto)
    {
        var activity = await db.Activities.Include(a => a.Constraints).FirstOrDefaultAsync(a => a.Id == id);
        if (activity is null) return NotFound();

        activity.Name = dto.Name;
        activity.Enabled = dto.Enabled;

        if (activity.Constraints is null)
            activity.Constraints = MapConstraints(dto.Constraints);
        else
            ApplyConstraints(activity.Constraints, dto.Constraints);

        await db.SaveChangesAsync();
        return Ok(MapToDto(activity));
    }

    [HttpPatch("{id:int}/enabled")]
    public async Task<IActionResult> ToggleEnabled(int id, [FromBody] bool enabled)
    {
        var activity = await db.Activities.FindAsync(id);
        if (activity is null) return NotFound();
        activity.Enabled = enabled;
        await db.SaveChangesAsync();
        return Ok(new { activity.Id, activity.Enabled });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var activity = await db.Activities.FindAsync(id);
        if (activity is null) return NotFound();
        db.Activities.Remove(activity);
        await db.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("templates")]
    public IActionResult GetTemplates() => Ok(ActivityTemplates.All);

    // --- Helpers ---

    private static ActivityDto MapToDto(Activity a) => new(
        a.Id, a.Name, a.TemplateSlug, a.Enabled, a.CreatedAt,
        a.Constraints is null ? null : new ConstraintsDto(
            a.Constraints.TempMin, a.Constraints.TempMax,
            a.Constraints.WindMin, a.Constraints.WindMax,
            a.Constraints.CloudMin, a.Constraints.CloudMax,
            a.Constraints.PrecipMax,
            a.Constraints.TimeStart, a.Constraints.TimeEnd,
            a.Constraints.DaysOfWeek
        )
    );

    private static WeatherConstraints MapConstraints(ConstraintsDto dto) => new()
    {
        TempMin = dto.TempMin, TempMax = dto.TempMax,
        WindMin = dto.WindMin, WindMax = dto.WindMax,
        CloudMin = dto.CloudMin, CloudMax = dto.CloudMax,
        PrecipMax = dto.PrecipMax,
        TimeStart = dto.TimeStart, TimeEnd = dto.TimeEnd,
        DaysOfWeek = dto.DaysOfWeek,
    };

    private static void ApplyConstraints(WeatherConstraints wc, ConstraintsDto dto)
    {
        wc.TempMin = dto.TempMin; wc.TempMax = dto.TempMax;
        wc.WindMin = dto.WindMin; wc.WindMax = dto.WindMax;
        wc.CloudMin = dto.CloudMin; wc.CloudMax = dto.CloudMax;
        wc.PrecipMax = dto.PrecipMax;
        wc.TimeStart = dto.TimeStart; wc.TimeEnd = dto.TimeEnd;
        wc.DaysOfWeek = dto.DaysOfWeek;
    }
}

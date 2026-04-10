using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeatherActivityMatcher.Api.Data;
using WeatherActivityMatcher.Api.Dtos;

namespace WeatherActivityMatcher.Api.Controllers;

[ApiController]
[Route("api/v1/notifications")]
public class NotificationsController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] bool unreadOnly = false, [FromQuery] int limit = 50)
    {
        var query = db.Notifications
            .Include(n => n.Activity)
            .AsQueryable();

        if (unreadOnly)
            query = query.Where(n => !n.Read);

        var items = await query
            .OrderByDescending(n => n.CreatedAt)
            .Take(limit)
            .Select(n => new NotificationDto(
                n.Id, n.ActivityId, n.Activity.Name,
                n.WindowStart, n.WindowEnd,
                n.Message, n.Channel, n.Read, n.CreatedAt
            ))
            .ToListAsync();

        return Ok(items);
    }

    [HttpPatch("{id:int}/read")]
    public async Task<IActionResult> MarkRead(int id)
    {
        var n = await db.Notifications.FindAsync(id);
        if (n is null) return NotFound();
        n.Read = true;
        await db.SaveChangesAsync();
        return Ok(new { n.Id, n.Read });
    }

    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllRead()
    {
        await db.Notifications.Where(n => !n.Read).ExecuteUpdateAsync(s => s.SetProperty(n => n.Read, true));
        return Ok();
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> UnreadCount()
    {
        var count = await db.Notifications.CountAsync(n => !n.Read);
        return Ok(new { count });
    }
}

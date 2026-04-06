using WeatherActivityMatcher.Api.Data;
using WeatherActivityMatcher.Api.Models;

namespace WeatherActivityMatcher.Api.Services.Notifications;

public class InAppNotificationService(IServiceScopeFactory scopeFactory, ILogger<InAppNotificationService> logger)
    : INotificationService
{
    public string Channel => "inapp";

    public async Task SendAsync(NotificationPayload payload, CancellationToken ct = default)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var notification = new Notification
        {
            ActivityId = payload.ActivityId,
            WindowStart = payload.WindowStart,
            WindowEnd = payload.WindowEnd,
            Message = payload.Message,
            Channel = Channel,
            Read = false,
            CreatedAt = DateTime.UtcNow,
        };

        db.Notifications.Add(notification);
        await db.SaveChangesAsync(ct);
        logger.LogInformation("In-app notification created for activity {ActivityId}: {Message}", payload.ActivityId, payload.Message);
    }
}

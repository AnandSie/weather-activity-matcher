namespace WeatherActivityMatcher.Api.Services.Notifications;

public class NotificationRegistry(IEnumerable<INotificationService> services, ILogger<NotificationRegistry> logger)
{
    private readonly IReadOnlyList<INotificationService> _services = services.ToList();

    public async Task NotifyAllAsync(NotificationPayload payload, CancellationToken ct = default)
    {
        foreach (var svc in _services)
        {
            try
            {
                await svc.SendAsync(payload, ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Notification service '{Channel}' failed for activity {ActivityId}", svc.Channel, payload.ActivityId);
            }
        }
    }
}

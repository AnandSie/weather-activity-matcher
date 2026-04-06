namespace WeatherActivityMatcher.Api.Services.Notifications;

public interface INotificationService
{
    string Channel { get; }
    Task SendAsync(NotificationPayload payload, CancellationToken ct = default);
}

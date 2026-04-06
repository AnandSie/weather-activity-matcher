namespace WeatherActivityMatcher.Api.Services.Notifications;

public record NotificationPayload(
    int ActivityId,
    string ActivityName,
    DateTime WindowStart,
    DateTime WindowEnd,
    string Message
);

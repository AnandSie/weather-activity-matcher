namespace WeatherActivityMatcher.Api.Dtos;

public record NotificationDto(
    int Id,
    int ActivityId,
    string ActivityName,
    DateTime WindowStart,
    DateTime WindowEnd,
    string Message,
    string Channel,
    bool Read,
    DateTime CreatedAt
);

public record SettingsDto(
    double Latitude,
    double Longitude,
    string LocationName
);

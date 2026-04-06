namespace WeatherActivityMatcher.Api.Dtos;

public record ForecastHourDto(
    DateTime ValidTime,
    double Temperature2m,
    double Windspeed10m,
    double Precipitation,
    double Cloudcover
);

public record MatchWindowDto(
    int ActivityId,
    string ActivityName,
    DateTime WindowStart,
    DateTime WindowEnd,
    IReadOnlyList<ForecastHourDto> Hours
);

namespace WeatherActivityMatcher.Api.Models;

public class WeatherConstraints
{
    public int Id { get; set; }
    public int ActivityId { get; set; }
    public Activity Activity { get; set; } = null!;

    // Temperature in °C
    public double? TempMin { get; set; }
    public double? TempMax { get; set; }

    // Wind speed in km/h
    public double? WindMin { get; set; }
    public double? WindMax { get; set; }

    // Cloud cover 0–100 %
    public double? CloudMin { get; set; }
    public double? CloudMax { get; set; }

    // Precipitation mm/h — null = no constraint, 0 = no rain at all
    public double? PrecipMax { get; set; }

    // Time-of-day window stored as "HH:mm" strings
    public string? TimeStart { get; set; }
    public string? TimeEnd { get; set; }

    // Days-of-week bitmask: Monday=1, Tuesday=2, Wednesday=4, Thursday=8,
    // Friday=16, Saturday=32, Sunday=64. null = any day.
    public int? DaysOfWeek { get; set; }
}

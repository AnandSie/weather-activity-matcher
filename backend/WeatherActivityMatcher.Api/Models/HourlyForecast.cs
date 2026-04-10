namespace WeatherActivityMatcher.Api.Models;

public class HourlyForecast
{
    public int Id { get; set; }

    /// <summary>UTC hour this forecast applies to.</summary>
    public DateTime ValidTime { get; set; }

    public double Temperature2m { get; set; }   // °C
    public double Windspeed10m { get; set; }     // km/h
    public double Precipitation { get; set; }    // mm/h
    public double Cloudcover { get; set; }       // 0–100 %

    public DateTime FetchedAt { get; set; } = DateTime.UtcNow;
}

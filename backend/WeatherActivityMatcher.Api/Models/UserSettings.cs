namespace WeatherActivityMatcher.Api.Models;

public class UserSettings
{
    // Singleton row — always Id = 1
    public int Id { get; set; } = 1;
    public double Latitude { get; set; } = 52.37;   // Amsterdam default
    public double Longitude { get; set; } = 4.90;
    public string LocationName { get; set; } = "Amsterdam";
}

namespace WeatherActivityMatcher.Api.Models;

public class Notification
{
    public int Id { get; set; }
    public int ActivityId { get; set; }
    public Activity Activity { get; set; } = null!;

    public DateTime WindowStart { get; set; }
    public DateTime WindowEnd { get; set; }
    public string Message { get; set; } = "";
    public string Channel { get; set; } = "inapp";
    public bool Read { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

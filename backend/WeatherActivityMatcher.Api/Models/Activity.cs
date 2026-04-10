namespace WeatherActivityMatcher.Api.Models;

public class Activity
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? TemplateSlug { get; set; }
    public bool Enabled { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public WeatherConstraints? Constraints { get; set; }
    public ICollection<Notification> Notifications { get; set; } = [];
}

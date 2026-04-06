using Microsoft.EntityFrameworkCore;
using WeatherActivityMatcher.Api.Data;
using WeatherActivityMatcher.Api.Dtos;
using WeatherActivityMatcher.Api.Models;
using WeatherActivityMatcher.Api.Services.Notifications;

namespace WeatherActivityMatcher.Api.Services;

public class MatcherService(IServiceScopeFactory scopeFactory, NotificationRegistry notificationRegistry, ILogger<MatcherService> logger)
{
    public async Task<IReadOnlyList<MatchWindowDto>> GetMatchesAsync(CancellationToken ct = default)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var now = DateTime.UtcNow;
        var forecasts = await db.HourlyForecasts
            .Where(f => f.ValidTime >= now)
            .OrderBy(f => f.ValidTime)
            .ToListAsync(ct);

        var activities = await db.Activities
            .Include(a => a.Constraints)
            .Where(a => a.Enabled && a.Constraints != null)
            .ToListAsync(ct);

        var results = new List<MatchWindowDto>();

        foreach (var activity in activities)
        {
            var matchingHours = forecasts
                .Where(f => EvaluateConstraints(activity.Constraints!, f))
                .ToList();

            var windows = MergeConsecutiveHours(matchingHours);
            foreach (var window in windows)
            {
                results.Add(new MatchWindowDto(
                    ActivityId: activity.Id,
                    ActivityName: activity.Name,
                    WindowStart: window.Start,
                    WindowEnd: window.End,
                    Hours: window.Hours.Select(h => new ForecastHourDto(
                        h.ValidTime, h.Temperature2m, h.Windspeed10m, h.Precipitation, h.Cloudcover
                    )).ToList()
                ));
            }
        }

        return results;
    }

    public async Task RunMatchingAsync(CancellationToken ct = default)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var now = DateTime.UtcNow;
        var forecasts = await db.HourlyForecasts
            .Where(f => f.ValidTime >= now)
            .OrderBy(f => f.ValidTime)
            .ToListAsync(ct);

        var activities = await db.Activities
            .Include(a => a.Constraints)
            .Where(a => a.Enabled && a.Constraints != null)
            .ToListAsync(ct);

        var dedup24h = now.AddHours(-24);

        foreach (var activity in activities)
        {
            var matchingHours = forecasts
                .Where(f => EvaluateConstraints(activity.Constraints!, f))
                .ToList();

            var windows = MergeConsecutiveHours(matchingHours);

            foreach (var window in windows)
            {
                // Dedup: skip if already notified for this window in last 24 hours
                var alreadyNotified = await db.Notifications.AnyAsync(n =>
                    n.ActivityId == activity.Id &&
                    n.WindowStart == window.Start &&
                    n.CreatedAt >= dedup24h, ct);

                if (alreadyNotified) continue;

                var duration = window.End - window.Start;
                var message = $"{activity.Name}: good conditions {window.Start:ddd d MMM HH:mm}–{window.End:HH:mm} UTC ({duration.TotalHours:0}h)";

                var payload = new NotificationPayload(
                    ActivityId: activity.Id,
                    ActivityName: activity.Name,
                    WindowStart: window.Start,
                    WindowEnd: window.End,
                    Message: message
                );

                await notificationRegistry.NotifyAllAsync(payload, ct);
            }
        }

        logger.LogInformation("Matching run complete");
    }

    internal static bool EvaluateConstraints(WeatherConstraints c, HourlyForecast f)
    {
        var hour = f.ValidTime.Hour;
        // DayOfWeek: Sunday=0 in .NET, we use Mon=1…Sun=64 bitmask
        var dotNetDow = (int)f.ValidTime.DayOfWeek; // 0=Sun..6=Sat
        // Convert to our bitmask: Mon=1,Tue=2,Wed=4,Thu=8,Fri=16,Sat=32,Sun=64
        int dayBit = dotNetDow == 0 ? 64 : (1 << (dotNetDow - 1));

        return
            (c.TempMin is null || f.Temperature2m >= c.TempMin) &&
            (c.TempMax is null || f.Temperature2m <= c.TempMax) &&
            (c.WindMin is null || f.Windspeed10m >= c.WindMin) &&
            (c.WindMax is null || f.Windspeed10m <= c.WindMax) &&
            (c.CloudMin is null || f.Cloudcover >= c.CloudMin) &&
            (c.CloudMax is null || f.Cloudcover <= c.CloudMax) &&
            (c.PrecipMax is null || f.Precipitation <= c.PrecipMax) &&
            InTimeWindow(c.TimeStart, c.TimeEnd, hour) &&
            (c.DaysOfWeek is null || (c.DaysOfWeek & dayBit) != 0);
    }

    private static bool InTimeWindow(string? start, string? end, int hour)
    {
        if (start is null && end is null) return true;
        var startHour = start is not null ? int.Parse(start.Split(':')[0]) : 0;
        var endHour = end is not null ? int.Parse(end.Split(':')[0]) : 23;
        return hour >= startHour && hour < endHour;
    }

    private static IReadOnlyList<(DateTime Start, DateTime End, List<HourlyForecast> Hours)> MergeConsecutiveHours(
        List<HourlyForecast> hours)
    {
        if (hours.Count == 0) return [];

        var windows = new List<(DateTime Start, DateTime End, List<HourlyForecast> Hours)>();
        var currentHours = new List<HourlyForecast> { hours[0] };

        for (int i = 1; i < hours.Count; i++)
        {
            var gap = hours[i].ValidTime - hours[i - 1].ValidTime;
            if (gap <= TimeSpan.FromHours(1))
            {
                currentHours.Add(hours[i]);
            }
            else
            {
                windows.Add((currentHours[0].ValidTime, currentHours[^1].ValidTime.AddHours(1), currentHours));
                currentHours = [hours[i]];
            }
        }
        windows.Add((currentHours[0].ValidTime, currentHours[^1].ValidTime.AddHours(1), currentHours));

        return windows;
    }
}

using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using WeatherActivityMatcher.Api.Data;
using WeatherActivityMatcher.Api.Models;

namespace WeatherActivityMatcher.Api.Services;

public class WeatherService(IHttpClientFactory httpClientFactory, IServiceScopeFactory scopeFactory, ILogger<WeatherService> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task FetchAndStoreAsync(CancellationToken ct = default)
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var settings = await db.UserSettings.FindAsync([1], ct) ?? new UserSettings();
        var lat = settings.Latitude;
        var lon = settings.Longitude;

        logger.LogInformation("Fetching weather for {Lat},{Lon}", lat, lon);

        var url = $"https://api.open-meteo.com/v1/forecast" +
                  $"?latitude={lat}&longitude={lon}" +
                  $"&hourly=temperature_2m,windspeed_10m,precipitation,cloudcover" +
                  $"&forecast_days=7&timezone=UTC&windspeed_unit=kmh";

        var client = httpClientFactory.CreateClient("openmeteo");
        using var response = await client.GetAsync(url, ct);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(ct);
        var root = JsonSerializer.Deserialize<OpenMeteoResponse>(json, JsonOptions)
                   ?? throw new InvalidOperationException("Empty Open-Meteo response");

        var hourly = root.Hourly;
        var now = DateTime.UtcNow;

        // Build rows from parallel arrays
        var rows = hourly.Time.Select((timeStr, i) => new HourlyForecast
        {
            ValidTime = DateTime.Parse(timeStr, null, System.Globalization.DateTimeStyles.AssumeUniversal).ToUniversalTime(),
            Temperature2m = hourly.Temperature2m[i],
            Windspeed10m = hourly.Windspeed10m[i],
            Precipitation = hourly.Precipitation[i],
            Cloudcover = hourly.Cloudcover[i],
            FetchedAt = now,
        }).ToList();

        // Upsert: update existing or insert new
        var existingTimes = (await db.HourlyForecasts
            .Select(f => f.ValidTime)
            .ToListAsync(ct)).ToHashSet();

        foreach (var row in rows)
        {
            if (existingTimes.Contains(row.ValidTime))
            {
                var existing = await db.HourlyForecasts.FirstAsync(f => f.ValidTime == row.ValidTime, ct);
                existing.Temperature2m = row.Temperature2m;
                existing.Windspeed10m = row.Windspeed10m;
                existing.Precipitation = row.Precipitation;
                existing.Cloudcover = row.Cloudcover;
                existing.FetchedAt = now;
            }
            else
            {
                db.HourlyForecasts.Add(row);
            }
        }

        // Clean up old rows (older than 2 hours ago)
        var cutoff = now.AddHours(-2);
        await db.HourlyForecasts.Where(f => f.ValidTime < cutoff).ExecuteDeleteAsync(ct);

        await db.SaveChangesAsync(ct);
        logger.LogInformation("Stored {Count} hourly forecast rows", rows.Count);
    }

    // Open-Meteo response shape
    private sealed record OpenMeteoResponse(OpenMeteoHourly Hourly);
    private sealed record OpenMeteoHourly(
        string[] Time,
        double[] Temperature2m,
        double[] Windspeed10m,
        double[] Precipitation,
        double[] Cloudcover
    );
}

using WeatherActivityMatcher.Api.Services;

namespace WeatherActivityMatcher.Api.BackgroundServices;

public class WeatherRefreshBackgroundService(WeatherService weatherService, ILogger<WeatherRefreshBackgroundService> logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromHours(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Run immediately on startup, then on interval
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await weatherService.FetchAndStoreAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Weather refresh failed");
            }

            await Task.Delay(Interval, stoppingToken);
        }
    }
}

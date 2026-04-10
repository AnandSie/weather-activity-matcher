using WeatherActivityMatcher.Api.Services;

namespace WeatherActivityMatcher.Api.BackgroundServices;

public class MatchingBackgroundService(MatcherService matcherService, ILogger<MatchingBackgroundService> logger) : BackgroundService
{
    private static readonly TimeSpan InitialDelay = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan Interval = TimeSpan.FromHours(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Small delay so weather refresh runs first on startup
        await Task.Delay(InitialDelay, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await matcherService.RunMatchingAsync(stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Matching run failed");
            }

            await Task.Delay(Interval, stoppingToken);
        }
    }
}

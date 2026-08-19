using Core.Cache.Rehydration.Abstractions;
using Core.Cache.Rehydration.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Core.Cache.Rehydration.Background;

internal sealed class RehydrationBackgroundService(
    IRehydrationService rehydrationService,
    ILogger<RehydrationBackgroundService> logger,
    RehydrationOptions options)
    : BackgroundService
{
    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        logger.LogInformation(
            "Cache rehydration background service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await rehydrationService.ExecuteCycleAsync(
                    stoppingToken);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "An error occurred during cache rehydration.");
            }

            await Task.Delay(
                options.Interval,
                stoppingToken);
        }

        logger.LogInformation(
            "Cache rehydration background service stopped.");
    }
}
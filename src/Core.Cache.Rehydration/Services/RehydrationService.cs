using Core.Cache.Abstractions;
using Core.Cache.Rehydration.Abstractions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace Core.Cache.Rehydration.Services;

internal sealed class RehydrationService(
    //IPrimaryHealthState primaryHealthState,
    HealthCheckService healthCheckService,
    ICacheRehydrator rehydrator,
    ILogger<RehydrationService> logger)
    : IRehydrationService
{
    private bool _wasPrimaryUnavailable;

    public async Task ExecuteCycleAsync(
        CancellationToken cancellationToken)
    {
        var report =
            await healthCheckService.CheckHealthAsync(
                cancellationToken);

        var primaryChecks =
            report.Entries
                .Where(x => x.Value.Tags.Contains("primary"))
                .ToList();

        var primaryHealthy =
            primaryChecks.Count > 0 &&
            primaryChecks.All(
                x => x.Value.Status == HealthStatus.Healthy);

        if (primaryHealthy)
        {
            if (_wasPrimaryUnavailable)
            {
                logger.LogInformation(
                    "Primary cache recovered. Starting cache rehydration.");

                await rehydrator.RehydrateAsync(
                    cancellationToken);

                _wasPrimaryUnavailable = false;
            }
        }
        else
        {
            _wasPrimaryUnavailable = true;
        }
    }
}
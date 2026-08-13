using Core.Cache.Abstractions;
using Core.Cache.Rehydration.Abstractions;
using Microsoft.Extensions.Logging;

namespace Core.Cache.Rehydration.Services;

internal sealed class RehydrationService(
    IPrimaryHealthState primaryHealthState,
    ICacheRehydrator rehydrator,
    ILogger<RehydrationService> logger)
    : IRehydrationService
{
    private bool _wasPrimaryUnavailable;

    public async Task ExecuteCycleAsync(
        CancellationToken cancellationToken)
    {
        var primaryHealthy =
            primaryHealthState.IsHealthy;

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
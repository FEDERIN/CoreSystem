using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace Core.Cache.Services.Rehydration;

internal sealed class RedisRehydrationService(
    HealthCheckService healthCheckService,
    ICacheRehydrator rehydrator,
    ILogger<RedisRehydrationService> logger)
    : IRedisRehydrationService
{
    private bool _wasRedisDown;

    public async Task ExecuteCycleAsync(CancellationToken ct)
    {
        var report =
            await healthCheckService.CheckHealthAsync(ct);

        bool redisHealthy =
            report.Entries.TryGetValue(
                "redis_cache",
                out var entry) &&
            entry.Status == HealthStatus.Healthy;

        if (redisHealthy)
        {
            if (_wasRedisDown)
            {
                logger.LogInformation(
                    "Redis recovered. Starting cache rehydration.");

                await rehydrator.RehydrateAsync(ct);

                _wasRedisDown = false;
            }
        }
        else
        {
            _wasRedisDown = true;
        }
    }
}
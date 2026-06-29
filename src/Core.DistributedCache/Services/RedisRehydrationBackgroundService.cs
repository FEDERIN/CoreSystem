using Core.DistributedCache.Abstractions;
using Core.DistributedCache.Storage.Memory;
using Core.DistributedCache.Storage.Redis;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Core.DistributedCache.Services;

internal sealed class RedisRehydrationBackgroundService(
    HealthCheckService healthCheckService,
    MemoryStorage memoryStorage,
    RedisStorage redisStorage,
    ILogger<RedisRehydrationBackgroundService> logger)
    : BackgroundService
{
    private bool _wasRedisDown;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var report = await healthCheckService.CheckHealthAsync(stoppingToken);

            bool redisHealthy =
                report.Entries.TryGetValue("redis_cache", out var entry) &&
                entry.Status == HealthStatus.Healthy;

            if (redisHealthy)
            {
                if (_wasRedisDown)
                {
                    logger.LogInformation(
                        "Redis recovered. Starting cache rehydration.");

                    await RehydrateAsync(stoppingToken);

                    _wasRedisDown = false;
                }
            }
            else
            {
                _wasRedisDown = true;
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }

    private async Task RehydrateAsync(CancellationToken ct)
    {
        const int BatchSize = 100;

        var keys = memoryStorage.GetTrackedKeys().ToList();

        foreach (var batch in keys.Chunk(BatchSize))
        {
            foreach (var key in batch)
            {
                ct.ThrowIfCancellationRequested();

                var wrapper = memoryStorage.GetWrapper<object>(key);

                if (wrapper is null)
                    continue;

                if (wrapper.Origin != CacheProviderType.Redis)
                    continue;

                try
                {
                    await redisStorage.SetAsync(
                        key,
                        wrapper.Value,
                        ct: ct);

                    await memoryStorage.RemoveAsync(
                        key,
                        ct);
                }
                catch (Exception ex)
                {
                    logger.LogError(
                        ex,
                        "Unable to rehydrate cache key '{Key}'. It will be retried later.",
                        key);
                }
            }

            await Task.Delay(100, ct);
        }
    }
}
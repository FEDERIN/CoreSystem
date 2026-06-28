using Core.DistributedCache.Abstractions;
using Core.DistributedCache.Storage.Memory;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Core.DistributedCache.Services;

internal class RedisRehydrationBackgroundService(
    HealthCheckService healthCheckService,
    MemoryCacheStorage memoryCache,
    ICoreCacheService redisCache,
    ILogger<RedisRehydrationBackgroundService> logger) : BackgroundService
{
    private bool _wasRedisDown = false;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var report = await healthCheckService.CheckHealthAsync(stoppingToken);
            bool isRedisHealthy = report.Entries.TryGetValue("redis_cache", out var entry)
                                  && entry.Status == HealthStatus.Healthy;

            if (isRedisHealthy && _wasRedisDown)
            {
                logger.LogInformation("Redis recovered. Starting rehydration...");
                await RehydrateAsync();
                _wasRedisDown = false;
            }
            else if (!isRedisHealthy)
            {
                _wasRedisDown = true;
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }

    private async Task RehydrateAsync()
    {
        const int batchSize = 100;
        var keys = memoryCache.GetTrackedKeys().ToList();

        foreach (var chunk in keys.Chunk(batchSize))
        {
            foreach (var key in chunk)
            {
                var wrapper = memoryCache.GetWrapper<object>(key);

                // Only migrate data that was originally intended for Redis
                if (wrapper?.Origin == CacheProviderType.Redis)
                {
                    try
                    {
                        await redisCache.SetAsync(key, wrapper.Value);
                        await memoryCache.RemoveAsync(key);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error rehydrating key {Key}. It will be kept in memory for the next attempt.", key);
                    }
                }
            }
            // Small throttle to avoid overwhelming Redis during recovery
            await Task.Delay(100);
        }
    }
}
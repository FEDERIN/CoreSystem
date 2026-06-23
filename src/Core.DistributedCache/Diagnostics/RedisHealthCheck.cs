using Core.DistributedCache.Abstractions;
using Core.DistributedCache.Storage;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace Core.DistributedCache.Diagnostics;

internal class RedisHealthCheck(IConnectionMultiplexer redis, ICoreCacheService cacheService) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken ct = default)
    {
        // Check if the cache service is using the ResilientCacheDecorator
        if (cacheService is ResilientCacheDecorator decorator)
        {
            if (!decorator.IsRedisHealthy)
            {
                return HealthCheckResult.Degraded("Redis reported as down by the decorator (Circuit Breaker open).");
            }
        }

        try
        {
            var db = redis.GetDatabase();
            await db.PingAsync();

            return HealthCheckResult.Healthy("Redis is connected successfully.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Degraded("Redis is not responding. Memory fallback active.", ex);
        }
    }
}
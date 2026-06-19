using Core.DistributedCache.Abstractions;
using Core.DistributedCache.Storage;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace Core.DistributedCache.Diagnostics;

public class RedisHealthCheck(IConnectionMultiplexer redis, ICoreCacheService cacheService) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken ct = default)
    {
        if (cacheService is ResilientCacheDecorator decorator)
        {
            if (!decorator.IsRedisHealthy)
            {
                return HealthCheckResult.Degraded("Redis reportado como caído por el decorador (Circuit Breaker abierto).");
            }
        }

        try
        {
            var db = redis.GetDatabase();
            await db.PingAsync();

            return HealthCheckResult.Healthy("Redis está conectado correctamente.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Degraded("Redis no responde. Fallback a memoria activo.", ex);
        }
    }
}
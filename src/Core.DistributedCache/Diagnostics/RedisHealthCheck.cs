using Core.Cache.Abstractions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace Core.Cache.Diagnostics;

internal sealed class RedisHealthCheck(
    IConnectionMultiplexer redis,
    IRedisHealthState healthState)
    : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken ct = default)
    {
        if (!healthState.IsRedisHealthy)
        {
            return HealthCheckResult.Degraded(
                "Redis is unavailable. Circuit breaker is open and the cache is operating with the fallback storage.");
        }

        try
        {
            var db = redis.GetDatabase();

            await db.PingAsync();

            return HealthCheckResult.Healthy(
                "Redis is connected successfully.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Degraded(
                "Redis is not responding. Memory fallback active.",
                ex);
        }
    }
}
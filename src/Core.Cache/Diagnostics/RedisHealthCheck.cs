using Core.Cache.Abstractions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Core.Cache.Diagnostics;

internal sealed class RedisHealthCheck(
    IConnectionMultiplexer redis,
    IHealthState healthState,
    ILogger<RedisHealthCheck> logger)
    : IHealthCheck
{
    private const string RedisRecoveredMessage =
    "Redis connection restored.";

    private const string RedisUnavailableMessage =
        "Redis became unavailable. Switching to memory fallback.";

    private const string RedisUnavailableHealthMessage =
        "Redis is not responding. Memory fallback active.";

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken ct = default)
    {
        try
        {
            await redis.GetDatabase().PingAsync();

            if (healthState.Update(true) == HealthTransition.BecameHealthy)
            {
                logger.LogInformation(
                    RedisRecoveredMessage);
            }

            return HealthCheckResult.Healthy(
                "Redis is connected successfully.");
        }
        catch (Exception ex)
        {
            if (healthState.Update(false) == HealthTransition.BecameUnhealthy)
            {
                logger.LogWarning(
                    ex,
                    RedisUnavailableMessage);
            }

            return HealthCheckResult.Degraded(
                RedisUnavailableHealthMessage,
                ex);
        }
    }
}
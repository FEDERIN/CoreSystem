using Core.Cache.Abstractions;

namespace Core.Cache.Diagnostics;

internal sealed class RedisHealthState : IHealthState
{
    private volatile bool _healthy = true;

    public bool IsRedisHealthy => _healthy;

    public HealthTransition Update(bool healthy)
    {
        if (_healthy == healthy)
            return HealthTransition.None;

        _healthy = healthy;

        return healthy
            ? HealthTransition.BecameHealthy
            : HealthTransition.BecameUnhealthy;
    }
}
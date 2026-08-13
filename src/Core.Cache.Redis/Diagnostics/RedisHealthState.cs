using Core.Cache.Abstractions;
using Core.Cache.Redis.Storage.Abstractions;

namespace Core.Cache.Redis.Diagnostics;

internal sealed class RedisHealthState : 
    IHealthState,
    IPrimaryHealthState,
    IPrimaryHealthStateWriter
{
    private volatile bool _healthy = true;

    public bool IsRedisHealthy => _healthy;

    public bool IsHealthy => _healthy;

    public HealthTransition Update(bool healthy)
    {
        if (_healthy == healthy)
            return HealthTransition.None;

        _healthy = healthy;

        return healthy
            ? HealthTransition.BecameHealthy
            : HealthTransition.BecameUnhealthy;
    }
    public void MarkUnavailable()
    {
        _healthy = false;
    }
}
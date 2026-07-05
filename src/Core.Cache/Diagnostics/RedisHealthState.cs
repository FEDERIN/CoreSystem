using Core.Cache.Abstractions;

namespace Core.Cache.Diagnostics;

internal sealed class RedisHealthState : IHealthState
{
    private volatile bool _healthy = true;
    public bool IsRedisHealthy => _healthy;
    public void MarkHealthy() => _healthy = true;
    public void MarkUnhealthy() => _healthy = false;
}
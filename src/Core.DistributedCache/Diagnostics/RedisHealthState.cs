using Core.DistributedCache.Abstractions;

namespace Core.DistributedCache.Diagnostics;

internal sealed class RedisHealthState : IRedisHealthState
{
    private volatile bool _healthy = true;
    public bool IsRedisHealthy => _healthy;
    public void MarkHealthy() => _healthy = true;
    public void MarkUnhealthy() => _healthy = false;
}
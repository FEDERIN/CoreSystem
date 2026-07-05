using Core.Cache.Abstractions;

namespace Core.Cache.Diagnostics;

internal sealed class NoOpHealthState : IHealthState
{
    public bool IsRedisHealthy => false;

    public void MarkHealthy()
    {
    }

    public void MarkUnhealthy()
    {
    }
}

using Core.Cache.Abstractions;

namespace Core.Cache.Diagnostics;

internal sealed class NoOpHealthState : IHealthState
{
    public bool IsRedisHealthy => false;

    public HealthTransition Update(bool healthy)
        => HealthTransition.None;
}
namespace Core.Cache.Abstractions;

internal interface IHealthState
{
    bool IsRedisHealthy { get; }

    HealthTransition Update(bool healthy);

}
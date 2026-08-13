using Core.Cache.Redis.Diagnostics;

namespace Core.Cache.Redis.Storage.Abstractions;

internal interface IHealthState
{
    bool IsRedisHealthy { get; }

    HealthTransition Update(bool healthy);

}
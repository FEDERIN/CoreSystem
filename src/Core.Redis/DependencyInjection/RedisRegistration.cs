using Core.Redis.Connection;
using Core.Redis.Synchronization;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Redis.DependencyInjection;

public static class RedisRegistration
{
    public static IServiceCollection AddCoreRedis(
        this IServiceCollection services)
    {
        services.AddSingleton<IRedisConnectionFactory, RedisConnectionFactory>();
        services.AddSingleton<IDistributedLockProvider, RedisLockProvider>();

        return services;
    }
}
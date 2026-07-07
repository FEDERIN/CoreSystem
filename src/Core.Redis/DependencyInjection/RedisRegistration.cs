using Core.Redis.Connection;
using Core.Redis.Options;
using Core.Redis.Synchronization;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Redis.DependencyInjection;

public static class RedisRegistration
{
    public static IServiceCollection  AddCoreRedis(
        this IServiceCollection services,
        Action<RedisLockOptions>? configure = null)
    {
        var options = new RedisLockOptions();

        configure?.Invoke(options);

        services.AddSingleton(options);
        services.AddSingleton<IRedisConnectionFactory, RedisConnectionFactory>();
        services.AddSingleton<IDistributedLockProvider, RedisLockProvider>();

        return services;
    }
}
using Core.Cache.Abstractions;
using Core.Cache.Diagnostics;
using Core.Cache.Options;
using Core.Cache.Services.Rehydration;
using Core.Cache.Storage.Abstractions;
using Core.Cache.Storage.Redis;
using Core.Cache.Storage.Redis.Abstractions;
using Core.Cache.Storage.Rehydration;
using Core.Observability.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Core.Cache.DependencyInjection;

internal static class RedisRegistration
{
    public static IServiceCollection AddCacheRedis(
    this IServiceCollection services, CacheOptions options)
    {
        if (!options.Redis.Enabled ||
            options.Redis.Configuration is null)
        {
            services.AddSingleton<IHealthState, NoOpHealthState>();
            return services;
        }

        services.AddSingleton<IRedisConnectionFactory, RedisConnectionFactory>();

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var factory = sp.GetRequiredService<IRedisConnectionFactory>();

            return factory.Create(options);
        });

        services.AddSingleton<IKeyBuilder, RedisKeyBuilder>();
        services.AddSingleton<IPayloadSerializer, PayloadSerializer>();
        services.AddSingleton<ICacheTagIndex<RedisStorage>, RedisTagIndex>();
        services.AddSingleton<ICacheLockProvider<RedisStorage>, RedisLockProvider>();


        services.AddSingleton<RedisStorage>(sp =>
            new RedisStorage(
                sp.GetRequiredService<IConnectionMultiplexer>(),
                sp.GetRequiredService<IPayloadSerializer>(),
                sp.GetRequiredService<IKeyBuilder>(),
                sp.GetRequiredService<ICacheTagIndex<RedisStorage>>(),
                sp.GetRequiredService<ICacheLockProvider<RedisStorage>>()
                ));

        // Diagnostics
        services.AddSingleton<IHealthState,
            RedisHealthState>();

        services.AddSingleton<RedisHealthCheck>();

        services.AddSingleton<IHealthCheckContributor,
            CacheHealthContributor>();


        if (options.DefaultProvider != CacheProviderType.Redis)
        {
            return services;
        }

        // Rehydration
        services.AddSingleton<IRehydrationSource, MemoryRehydrationSource>();
        services.AddSingleton<IRehydrationTarget, RedisRehydrationTarget>();
        services.AddSingleton<ICacheRehydrator, CacheRehydrator>();
        services.AddSingleton<IRedisRehydrationService, RedisRehydrationService>();

        services.AddHostedService<RedisRehydrationBackgroundService>();
        

        return services;
    }
}

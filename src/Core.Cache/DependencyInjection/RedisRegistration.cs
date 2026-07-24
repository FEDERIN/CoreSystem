using Core.Cache.Abstractions;
using Core.Cache.Diagnostics;
using Core.Cache.Options;
using Core.Cache.Services.Rehydration;
using Core.Cache.Storage.Abstractions;
using Core.Cache.Storage.Abstractions.Redis;
using Core.Cache.Storage.Redis;
using Core.Cache.Storage.Rehydration;
using Core.Observability.Abstractions;
using Core.Redis.Connection;
using Core.Redis.DependencyInjection;
using Core.Redis.Synchronization;
using Core.Serialization.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Core.Cache.DependencyInjection;

internal static class RedisRegistration
{
    public static IServiceCollection AddCacheRedis(
    this IServiceCollection services, CacheOptions options)
    {
        if (options.DefaultProvider != CacheProviderType.Redis ||
            !options.Redis.Enabled ||
            options.Redis.Configuration is null)
        {
            services.AddSingleton<IHealthState, NoOpHealthState>();
            return services;
        }

        services.AddCoreRedis();
        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var factory = sp.GetRequiredService<IRedisConnectionFactory>();

            return factory.Create(options.Redis.Configuration);
        });

        services.AddSingleton<IKeyBuilder, RedisKeyBuilder>();

        services.AddSingleton<RedisTagIndex>();

        services.AddSingleton<ICacheTagIndex<RedisStorage>>(sp =>
            sp.GetRequiredService<RedisTagIndex>());

        services.AddSingleton<IRedisTagIndex>(sp =>
            sp.GetRequiredService<RedisTagIndex>());

        services.AddSingleton<RedisStorage>(sp =>
            new RedisStorage(
                sp.GetRequiredService<IConnectionMultiplexer>(),
                sp.GetRequiredService<IPayloadSerializer>(),
                sp.GetRequiredService<IKeyBuilder>(),
                sp.GetRequiredService<ICacheTagIndex<RedisStorage>>(),
                sp.GetRequiredService<IDistributedLockProvider>(),
                sp.GetRequiredService<ILogger<RedisStorage>>()
            ));

        // Diagnostics
        services.AddSingleton<IHealthState,
            RedisHealthState>();

        services.AddSingleton<RedisHealthCheck>();

        services.AddSingleton<IHealthCheckContributor,
            CacheHealthContributor>();

        // Rehydration
        services.AddSingleton<IRehydrationSource, MemoryRehydrationSource>();
        services.AddSingleton<IRehydrationTarget, RedisRehydrationTarget>();
        services.AddSingleton<ICacheRehydrator, CacheRehydrator>();
        services.AddSingleton<IRedisRehydrationService, RedisRehydrationService>();

        services.AddHostedService<RedisRehydrationBackgroundService>();
        

        return services;
    }
}

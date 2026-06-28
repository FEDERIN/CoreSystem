using Core.DistributedCache.Abstractions;
using Core.DistributedCache.Diagnostics;
using Core.DistributedCache.Middleware;
using Core.DistributedCache.Options;
using Core.DistributedCache.Serialization;
using Core.DistributedCache.Services;
using Core.DistributedCache.Storage;
using Core.DistributedCache.Storage.Memory;
using Core.DistributedCache.Storage.Memory.Abstractions;
using Core.DistributedCache.Storage.Redis;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System.Diagnostics.Metrics;

namespace Core.DistributedCache;

public static class DistributedCacheExtensions
{
    public static IServiceCollection AddCoreDistributedCache(
        this IServiceCollection services,
        Action<CacheOptions> setupAction)
    {
        var options = new CacheOptions();
        setupAction(options);
        services.AddSingleton(options);

        services.AddSingleton<JsonCacheSerializer>();
        services.AddSingleton<MessagePackCacheSerializer>();
        services.AddSingleton<ProtobufCacheSerializer>();
        services.AddSingleton<ICacheSerializerFactory, CacheSerializerFactory>();

        services.AddSingleton(s => new CacheMetrics(
                s.GetRequiredService<IMeterFactory>()));

        services.AddMemoryCache();
        services.AddSingleton<IMemoryTagIndex, MemoryTagIndex>();
        services.AddSingleton<IMemoryKeyTracker, MemoryKeyTracker>();
        services.AddSingleton<IKeyLockProvider, KeyLockProvider>();

        services.AddSingleton<MemoryCacheStorage>();

        if (options.Redis?.Enabled == true && options.Redis?.Configuration != null)
        {
            var redisConfig = new ConfigurationOptions();
            options.Redis.Configuration(redisConfig);

            redisConfig.AbortOnConnectFail = false;
            redisConfig.ConnectRetry = 3;
            redisConfig.ConnectTimeout = 5000;

            var connection = ConnectionMultiplexer.Connect(redisConfig);
            services.AddSingleton<IConnectionMultiplexer>(connection);

            services.AddSingleton<RedisCacheStorage>(sp =>
            {
                var conn = sp.GetRequiredService<IConnectionMultiplexer>();
                return new RedisCacheStorage(conn, options, sp.GetRequiredService<ICacheSerializerFactory>());
            });

            services.AddSingleton<ICoreCacheService, ResilientCacheDecorator>();
        }
        else
        {
            services.AddSingleton<ICoreCacheService, MemoryCacheStorage>();
        }

        services.AddSingleton<ICacheServiceFactory, CacheServiceFactory>();

        services.AddHostedService<RedisRehydrationBackgroundService>();

        return services;
    }

    public static IApplicationBuilder UseCoreDistributedCache(this IApplicationBuilder app)
    {
        return app.UseMiddleware<CacheMiddleware>();
    }
}
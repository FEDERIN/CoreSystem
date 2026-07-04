using Core.Cache.Options;
using Core.Cache.Services.Rehydration;
using Core.Cache.Storage.Abstractions;
using Core.Cache.Storage.Redis;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Core.Cache.DependencyInjection;

internal static class RedisRegistration
{
    public static IServiceCollection AddCacheRedis(
    this IServiceCollection services, CacheOptions options)
    {
        if (options.Redis?.Enabled == true &&
            options.Redis.Configuration is not null)
        {
            var redisConfig = new ConfigurationOptions();

            options.Redis.Configuration(redisConfig);

            redisConfig.AbortOnConnectFail = false;
            redisConfig.ConnectRetry = 1;
            redisConfig.ConnectTimeout = 500;
            redisConfig.ReconnectRetryPolicy = new ExponentialRetry(1000);
            redisConfig.KeepAlive = 60;

            var connection = ConnectionMultiplexer.Connect(redisConfig);

            services.AddSingleton<IConnectionMultiplexer>(connection);
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


            services.AddSingleton<ICacheRehydrator, CacheRehydrator>();
            services.AddSingleton<IRedisRehydrationService, RedisRehydrationService>();
            services.AddHostedService<RedisRehydrationBackgroundService>();
        }

        return services;
    }
}

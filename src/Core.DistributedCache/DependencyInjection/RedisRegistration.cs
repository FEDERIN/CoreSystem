using Core.DistributedCache.Options;
using Core.DistributedCache.Services;
using Core.DistributedCache.Storage.Abstractions;
using Core.DistributedCache.Storage.Redis;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Core.DistributedCache.DependencyInjection;

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
            redisConfig.ConnectRetry = 3;
            redisConfig.ConnectTimeout = 5000;
            redisConfig.ReconnectRetryPolicy = new ExponentialRetry(5000);
            redisConfig.KeepAlive = 60;

            var connection = ConnectionMultiplexer.Connect(redisConfig);

            services.AddSingleton<IConnectionMultiplexer>(connection);
            services.AddSingleton<IKeyBuilder, RedisKeyBuilder>();
            services.AddSingleton<IPayloadSerializer, PayloadSerializer>();

            services.AddSingleton<RedisTagIndex>(sp =>
            {
                var db = sp.GetRequiredService<IConnectionMultiplexer>()
                           .GetDatabase();

                return new RedisTagIndex(
                    db,
                    sp.GetRequiredService<IKeyBuilder>());
            });

            services.AddSingleton<RedisLockProvider>(sp =>
            {
                return new RedisLockProvider(
                    sp.GetRequiredService<IConnectionMultiplexer>(),
                    sp.GetRequiredService<IKeyBuilder>());
            });

            services.AddSingleton<RedisStorage>(sp =>
                new RedisStorage(
                    sp.GetRequiredService<IConnectionMultiplexer>(),
                    sp.GetRequiredService<IPayloadSerializer>(),
                    sp.GetRequiredService<IKeyBuilder>(),
                    sp.GetRequiredService<RedisTagIndex>(),
                    sp.GetRequiredService<RedisLockProvider>()
                    ));

            services.AddHostedService<RedisRehydrationBackgroundService>();
        }

        return services;
    }
}

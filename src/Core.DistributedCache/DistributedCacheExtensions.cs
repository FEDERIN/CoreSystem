using Core.DistributedCache.Abstractions;
using Core.DistributedCache.Behaviors;
using Core.DistributedCache.Diagnostics;
using Core.DistributedCache.Middleware;
using Core.DistributedCache.Options;
using Core.DistributedCache.Pipeline;
using Core.DistributedCache.Serialization;
using Core.DistributedCache.Services;
using Core.DistributedCache.Storage;
using Core.DistributedCache.Storage.Memory;
using Core.DistributedCache.Storage.Memory.Abstractions;
using Core.DistributedCache.Storage.Redis;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using StackExchange.Redis;
using System.Diagnostics.Metrics;

namespace Core.DistributedCache;

public static class DistributedCacheExtensions
{
    public static IServiceCollection AddCoreDistributedCache(
        this IServiceCollection services,
        Action<CacheOptions> setupAction)
    {
        // Options
        var options = new CacheOptions();
        setupAction(options);

        services.AddSingleton(options);

        // Serializers
        services.AddSingleton<JsonCacheSerializer>();
        services.AddSingleton<MessagePackCacheSerializer>();
        services.AddSingleton<ProtobufCacheSerializer>();
        services.AddSingleton<ICacheSerializerFactory, CacheSerializerFactory>();

        // Metrics
        services.AddSingleton(sp =>
            new CacheMetrics(sp.GetRequiredService<IMeterFactory>()));

        // Memory infrastructure
        services.AddMemoryCache();
        services.AddSingleton<IMemoryTagIndex, MemoryTagIndex>();
        services.AddSingleton<IMemoryKeyTracker, MemoryKeyTracker>();
        services.AddSingleton<IKeyLockProvider, KeyLockProvider>();
        services.AddSingleton<MemoryStorage>();

        // Redis
        if (options.Redis?.Enabled == true &&
            options.Redis.Configuration is not null)
        {
            var redisConfig = new ConfigurationOptions();

            options.Redis.Configuration(redisConfig);

            redisConfig.AbortOnConnectFail = false;
            redisConfig.ConnectRetry = 3;
            redisConfig.ConnectTimeout = 5000;

            var connection = ConnectionMultiplexer.Connect(redisConfig);

            services.AddSingleton<IConnectionMultiplexer>(connection);

            services.AddSingleton<RedisStorage>(sp =>
                new RedisStorage(
                    sp.GetRequiredService<IConnectionMultiplexer>(),
                    options,
                    sp.GetRequiredService<ICacheSerializerFactory>()));

            services.AddHostedService<RedisRehydrationBackgroundService>();
        }

        // Health
        services.AddSingleton<IRedisHealthState, RedisHealthState>();

        // Resilience
        services.AddSingleton(sp =>
            new ResiliencePipelineBuilder()
                .AddRetry(new RetryStrategyOptions
                {
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromMilliseconds(200),
                    BackoffType = DelayBackoffType.Exponential,
                    ShouldHandle = new PredicateBuilder()
                        .Handle<RedisConnectionException>()
                        .Handle<RedisTimeoutException>()
                        .Handle<TimeoutException>()
                })
                .AddCircuitBreaker(new CircuitBreakerStrategyOptions
                {
                    FailureRatio = 0.5,
                    SamplingDuration = TimeSpan.FromSeconds(30),
                    MinimumThroughput = 10,
                    BreakDuration = TimeSpan.FromSeconds(15),
                    ShouldHandle = new PredicateBuilder()
                        .Handle<RedisConnectionException>()
                        .Handle<RedisTimeoutException>()
                        .Handle<TimeoutException>()
                })
                .Build());

        // Pipeline
        services.AddSingleton<LoggingBehavior>();
        services.AddSingleton<MetricsBehavior>();
        services.AddSingleton<FallbackBehavior>();
        services.AddSingleton<ResilienceBehavior>();

        services.AddSingleton<ICachePipeline>(sp =>
            new CachePipeline(
            [
                sp.GetRequiredService<LoggingBehavior>(),
                sp.GetRequiredService<MetricsBehavior>(),
                sp.GetRequiredService<FallbackBehavior>(),
                sp.GetRequiredService<ResilienceBehavior>()
            ]));

        // Storage
        services.AddSingleton<ICacheStorageResolver, CacheStorageResolver>();
        services.AddSingleton<ICacheServiceFactory, CacheServiceFactory>();

        // Public API
        services.AddSingleton<ICoreCacheService, CoreCacheService>();

        return services;
    }

    public static IApplicationBuilder UseCoreDistributedCache(this IApplicationBuilder app)
    {
        return app.UseMiddleware<CacheMiddleware>();
    }
}
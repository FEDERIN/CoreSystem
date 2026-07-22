using Core.Cache.Abstractions;
using Core.Cache.Http;
using Core.Cache.Options;
using Core.Resilience.Abstractions;
using Core.Resilience.Options;
using Core.Serialization.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Core.Cache.DependencyInjection;

public static class CacheRegistration
{
    public static IServiceCollection AddCoreCache(
        this IServiceCollection services,
        Action<CacheOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        var options = new CacheOptions();
        configure(options);

        services.AddSingleton(options);

        services
            .AddLogging()
            .AddCoreSerialization(serialization =>
            {
                serialization.DefaultSerializer = options.SerializerType;
            })
            .AddCacheDiagnostics()
            .AddCacheMemory()
            .AddCacheRedis(options);

        if (options.Redis.Enabled)
        {
            services.PostConfigure<ResilienceOptions>(ConfigureRedisDefaults);
        }

        services
            .AddCachePipeline(options)
            .AddCacheHttp()
            .AddCacheServices();

        return services;
    }

    public static IApplicationBuilder UseCoreCache(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        if (app.ApplicationServices.GetService<IHttpCacheHandler>() is null)
        {
            throw new InvalidOperationException(CacheMessages.MissingRegistration);
        }

        return app.UseMiddleware<CacheMiddleware>();
    }

    private static void ConfigureRedisDefaults(ResilienceOptions options)
    {
        if (options.ContainsPipeline(PipelineType.Redis))
        {
            ApplyRedisExceptions(options.GetPipeline(PipelineType.Redis));
            return;
        }

        options.AddPipeline(PipelineType.Redis, pipeline =>
        {
            pipeline.Retry = new RetryOptions
            {
                MaxRetryAttempts = 1,
                Delay = TimeSpan.FromMilliseconds(100),
                BackoffType = BackoffType.Constant
            };

            pipeline.CircuitBreaker = new CircuitBreakerOptions
            {
                FailureRatio = 0.5,
                SamplingDuration = TimeSpan.FromSeconds(30),
                MinimumThroughput = 2,
                BreakDuration = TimeSpan.FromSeconds(15)
            };

            pipeline.Timeout = new TimeoutOptions
            {
                Timeout = TimeSpan.FromSeconds(2)
            };

            ApplyRedisExceptions(pipeline);
        });
    }

    private static void ApplyRedisExceptions(PipelineOptions pipeline)
    {
        pipeline.Retry?
            .Handle<RedisConnectionException>()
            .Handle<RedisTimeoutException>()
            .Handle<TimeoutException>();

        pipeline.CircuitBreaker?
            .Handle<RedisConnectionException>()
            .Handle<RedisTimeoutException>()
            .Handle<TimeoutException>();
    }
}
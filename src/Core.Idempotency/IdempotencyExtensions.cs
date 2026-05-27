using Core.Idempotency.Middleware;
using Core.Idempotency.Options;
using Core.Idempotency.Storage;
using Core.Idempotency.Storage.Redis;
using Core.Idempotency.Storage.PostgreSQL;
using Core.Idempotency.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Core.Idempotency;

public static class IdempotencyExtensions
{
    private static IServiceCollection AddIdempotencyCore(this IServiceCollection services, Action<IdempotencyOptions>? setupAction)
    {
        // 1. Setup options
        var options = new IdempotencyOptions();
        setupAction?.Invoke(options);
        services.Configure(setupAction ?? (_ => { }));

        // 2. Register IdempotencyMetrics as a Singleton
        // We use the MeterName defined in IdempotencyOptions
        services.AddSingleton(new IdempotencyMetrics(options.MeterName));

        return services;
    }

    public static IServiceCollection AddRedisIdempotency(
            this IServiceCollection services,
            string redisConnectionString,
            Action<IdempotencyOptions>? setupAction = null)
    {
        services.AddIdempotencyCore(setupAction);

        var config = ConfigurationOptions.Parse(redisConnectionString);
        config.AbortOnConnectFail = false;

        services.AddSingleton<IConnectionMultiplexer>(sp => ConnectionMultiplexer.Connect(config));
        services.AddSingleton<IIdempotencyStorage, RedisIdempotencyStorage>();

        return services;
    }

    public static IServiceCollection AddPostgresIdempotency(
        this IServiceCollection services,
        string connectionString,
        Action<IdempotencyOptions>? setupAction = null)
    {
        services.AddIdempotencyCore(setupAction);

        services.AddSingleton<IIdempotencyStorage>(new PostgresIdempotencyStorage(connectionString));

        return services;
    }

    public static IApplicationBuilder UseIdempotency(this IApplicationBuilder app)
    {
        var storage = app.ApplicationServices.GetService<IIdempotencyStorage>();
        if (storage == null) return app;

        return app.UseMiddleware<IdempotencyMiddleware>();
    }
}
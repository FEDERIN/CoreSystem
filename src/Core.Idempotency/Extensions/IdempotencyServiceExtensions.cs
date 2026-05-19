using Core.Idempotency.Options;
using Core.Idempotency.Storage;
using Core.Idempotency.Storage.PostgreSQL;
using Core.Idempotency.Storage.Redis;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Core.Idempotency.Extensions;

public static class IdempotencyServiceExtensions
{
    private static IServiceCollection AddIdempotencyCore(this IServiceCollection services, Action<IdempotencyOptions>? setupAction)
    {
        services.Configure(setupAction ?? (_ => { }));

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

    public static IServiceCollection AddRedisIdempotency(
        this IServiceCollection services,
        string redisConnectionString,
        Action<IdempotencyOptions>? setupAction = null)
    {
        services.AddIdempotencyCore(setupAction);

        var multiplexer = ConnectionMultiplexer.Connect(redisConnectionString);
        services.AddSingleton<IConnectionMultiplexer>(multiplexer);

        services.AddSingleton<IIdempotencyStorage, RedisIdempotencyStorage>();

        return services;
    }
}
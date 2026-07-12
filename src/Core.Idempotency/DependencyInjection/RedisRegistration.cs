using Core.Idempotency.Abstractions;
using Core.Idempotency.Options;
using Core.Idempotency.Storage.Abstractions;
using Core.Idempotency.Storage.Redis;
using Core.Redis.Connection;
using Core.Redis.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Core.Idempotency.DependencyInjection;

internal static class RedisRegistration
{
    public static IServiceCollection AddIdempotencyRedis(
        this IServiceCollection services,
        IdempotencyOptions options)
    {
        if (options.Redis.Configuration is null)
        {
            return services;
        }

        services.AddCoreRedis();

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var factory =
                sp.GetRequiredService<IRedisConnectionFactory>();

            return factory.Create(options.Redis.Configuration);
        });

        services.AddSingleton<IKeyBuilder, RedisKeyBuilder>();
        services.AddSingleton<IIdempotencyStorage, RedisStorage>();

        return services;
    }
}
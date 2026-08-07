using Core.Idempotency.Abstractions;
using Core.Idempotency.Options;
using Core.Idempotency.Redis.Builders;
using Core.Idempotency.Redis.Options;
using Core.Idempotency.Redis.Storage;
using Core.Redis.Connection;
using Core.Redis.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Core.Idempotency.Redis.DependencyInjection;

internal static class IdempotencyRedisRegistration
{
    public static IServiceCollection AddRedisIdempotency(
        this IServiceCollection services,
        RedisOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (services.All(d => d.ServiceType != typeof(IdempotencyOptions)))
        {
            throw new InvalidOperationException(
                RedisMessages.IdempotencyRegistrationRequired);
        }

        if (options.Configuration is null)
        {
            throw new InvalidOperationException(
                RedisMessages.RedisConfigurationRequired);
        }

        services.AddCoreRedis();

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var factory =
                sp.GetRequiredService<IRedisConnectionFactory>();

            return factory.Create(options.Configuration);
        });

        services.AddSingleton<IKeyBuilder>(sp =>
        {
            var options = sp.GetRequiredService<IdempotencyOptions>();

            var prefix = string.IsNullOrWhiteSpace(options.InstanceName)
                ? string.Empty
                : $"{options.InstanceName}:";

            return new RedisKeyBuilder(prefix);
        });

        services.AddSingleton<IIdempotencyStorage, RedisIdempotencyStorage>();

        return services;
    }
}
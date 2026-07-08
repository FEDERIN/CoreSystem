using Core.Redis.Connection;
using Core.Redis.Options;
using Core.Redis.Synchronization;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Redis.DependencyInjection;

/// <summary>
/// Provides extension methods for registering Core.Redis services.
/// </summary>
public static class RedisRegistration
{
    /// <summary>
    /// Registers the Core.Redis infrastructure, including the connection
    /// factory and distributed lock provider.
    /// </summary>
    /// <param name="services">
    /// The service collection.
    /// </param>
    /// <param name="configure">
    /// Optional configuration for distributed lock behavior.
    /// </param>
    /// <returns>
    /// The updated service collection.
    /// </returns>
    public static IServiceCollection  AddCoreRedis(
        this IServiceCollection services,
        Action<RedisLockOptions>? configure = null)
    {
        var options = new RedisLockOptions();

        configure?.Invoke(options);

        services.AddSingleton(options);
        services.AddSingleton<IRedisConnectionFactory, RedisConnectionFactory>();
        services.AddSingleton<IDistributedLockProvider, RedisLockProvider>();

        return services;
    }
}
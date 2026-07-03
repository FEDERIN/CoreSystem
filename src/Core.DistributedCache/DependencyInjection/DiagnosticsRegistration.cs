using Core.DistributedCache.Abstractions;
using Core.DistributedCache.Diagnostics;
using Core.Observability.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Core.DistributedCache.DependencyInjection;

internal static class DiagnosticsRegistration
{
    public static IServiceCollection AddCacheDiagnostics(
        this IServiceCollection services)
    {
        services.AddMetrics();
        services.AddSingleton<CacheMetrics>();
        services.AddSingleton<RedisHealthCheck>();
        services.AddSingleton<IRedisHealthState, RedisHealthState>();
        services.AddSingleton<IHealthCheckContributor, CacheHealthContributor>();
        services.AddSingleton<IObservabilityContributor, CacheObservabilityContributor>();

        return services;
    }
}
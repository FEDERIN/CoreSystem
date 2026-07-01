using Core.DistributedCache.Abstractions;
using Core.DistributedCache.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.Metrics;

namespace Core.DistributedCache.DependencyInjection;

internal static class DiagnosticsRegistration
{
    public static IServiceCollection AddCacheDiagnostics(
        this IServiceCollection services)
    {
        // Metrics
        services.AddSingleton(sp =>
            new CacheMetrics(
                sp.GetRequiredService<IMeterFactory>()));

        // Health
        services.AddSingleton<IRedisHealthState, RedisHealthState>();

        return services;
    }
}
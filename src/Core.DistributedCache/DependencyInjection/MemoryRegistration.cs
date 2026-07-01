using Core.DistributedCache.Storage.Abstractions;
using Core.DistributedCache.Storage.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Core.DistributedCache.DependencyInjection;

internal static class MemoryRegistration
{
    public static IServiceCollection AddCacheMemory(
        this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<MemoryTagIndex>();
        services.AddSingleton<ICacheKeyTracker, MemoryKeyTracker>();
        services.AddSingleton<MemoryLockProvider>();
        services.AddSingleton<ICacheEntryFactory, CacheEntryFactory>();
        services.AddSingleton<MemoryStorage>();

        return services;
    }
}
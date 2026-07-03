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
        services.AddSingleton<ICacheTagIndex<MemoryStorage>, MemoryTagIndex>();
        services.AddSingleton<ICacheKeyTracker, MemoryKeyTracker>();
        services.AddSingleton<ICacheLockProvider<MemoryStorage>, MemoryLockProvider>();
        services.AddSingleton<ICacheEntryFactory, CacheEntryFactory>();
        services.AddSingleton<MemoryStorage>();

        return services;
    }
}
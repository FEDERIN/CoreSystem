using Core.Cache.Storage.Abstractions;
using Core.Cache.Storage.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Cache.DependencyInjection;

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
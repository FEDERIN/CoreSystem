using Core.Cache.Storage.Abstractions;
using Core.Cache.Storage.Memory;
using Core.Cache.Storage.Rehydration.Tracking;
using Core.Memory.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Cache.DependencyInjection;

internal static class MemoryRegistration
{
    public static IServiceCollection AddCacheMemory(
        this IServiceCollection services)
    {
        services.AddMemoryCache();

        // Entry
        services.AddSingleton<ICacheEntryFactory, CacheEntryFactory>();
        services.AddSingleton<ICacheEntryInspector, CacheEntryInspector>();

        // Tags
        services.AddSingleton<ICacheTagIndex<MemoryStorage>, MemoryTagIndex>();

        // Rehydration
        services.AddSingleton<ICacheKeyTracker, MemoryKeyTracker>();
        services.AddSingleton<IRehydrationTracker, RehydrationTracker>();

        services.AddCoreMemory();

        // Storage
        services.AddSingleton<MemoryStorage>();

        return services;
    }
}
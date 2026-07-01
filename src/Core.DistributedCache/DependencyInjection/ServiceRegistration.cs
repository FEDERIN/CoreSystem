using Core.DistributedCache.Abstractions;
using Core.DistributedCache.Services;
using Core.DistributedCache.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Core.DistributedCache.DependencyInjection;

internal static class ServiceRegistration
{
    public static IServiceCollection AddCacheServices(
        this IServiceCollection services)
    {
        services.AddSingleton<ICacheStorageResolver, CacheStorageResolver>();
        services.AddSingleton<ICacheServiceFactory, CacheServiceFactory>();
        services.AddSingleton<ICoreCacheService, CoreCacheService>();

        return services;
    }
}
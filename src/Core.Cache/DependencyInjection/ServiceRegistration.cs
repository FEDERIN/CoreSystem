using Core.Cache.Abstractions;
using Core.Cache.Services;
using Core.Cache.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Cache.DependencyInjection;

internal static class ServiceRegistration
{
    public static IServiceCollection AddCacheServices(
        this IServiceCollection services)
    {
        services.AddSingleton<ICacheStorageResolver, CacheStorageResolver>();
        services.AddSingleton<ICoreCache, CoreCache>();

        return services;
    }
}
using Core.DistributedCache.Abstractions;
using Core.DistributedCache.Storage.Memory;
using Core.DistributedCache.Storage.Redis;
using Microsoft.Extensions.DependencyInjection;

namespace Core.DistributedCache.Storage;

internal class CacheServiceFactory(IServiceProvider serviceProvider) : ICacheServiceFactory
{
    public ICoreCacheService GetDefaultCache()
    {
        return serviceProvider.GetRequiredService<ICoreCacheService>();
    }

    public ICoreCacheService GetCache(CacheProviderType type) => type switch
    {
        CacheProviderType.Memory => serviceProvider.GetRequiredService<MemoryCacheStorage>(),
        CacheProviderType.Redis => serviceProvider.GetService<RedisCacheStorage>()
                                               ?? throw new InvalidOperationException("Redis no está configurado"),
        _ => throw new NotSupportedException()
    };
}
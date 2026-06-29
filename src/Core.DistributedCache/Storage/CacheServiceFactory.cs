using Core.DistributedCache.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Core.DistributedCache.Storage;

internal sealed class CacheServiceFactory(
    IServiceProvider serviceProvider,
    ICacheStorageResolver resolver) : ICacheServiceFactory
{
    public ICoreCacheService GetDefaultCache()
    => serviceProvider.GetRequiredService<ICoreCacheService>();

    public ICacheStorage GetStorage(CacheProviderType type) => type switch
    {
        CacheProviderType.Redis => resolver.Primary,
        CacheProviderType.Memory => resolver.Fallback,
        _ => throw new NotSupportedException()
    };


}

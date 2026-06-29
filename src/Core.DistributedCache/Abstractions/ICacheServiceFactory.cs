
namespace Core.DistributedCache.Abstractions;

public interface ICacheServiceFactory
{
    ICoreCacheService GetDefaultCache();

    ICacheStorage GetStorage(CacheProviderType type);
}
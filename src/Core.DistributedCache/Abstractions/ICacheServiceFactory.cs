
namespace Core.DistributedCache.Abstractions;

public interface ICacheServiceFactory
{
    ICoreCacheService GetDefaultCache();
    ICoreCacheService GetCache(CacheProviderType type);
}
using Core.DistributedCache.Abstractions;
using Core.DistributedCache.Storage.Memory;
using Core.DistributedCache.Storage.Redis;

namespace Core.DistributedCache.Storage;

internal sealed class CacheStorageResolver : ICacheStorageResolver
{
    public CacheStorageResolver(
        MemoryStorage memoryStorage,
        RedisStorage? redisStorage = null)
    {
        if (redisStorage is not null)
        {
            Primary = redisStorage;
            Fallback = memoryStorage;
        }
        else
        {
            Primary = memoryStorage;
            Fallback = memoryStorage;
        }
    }

    public ICacheStorage Primary { get; }

    public ICacheStorage Fallback { get; }
}
using Core.Cache.Abstractions;
using Core.Cache.Storage.Memory;
using Core.Cache.Storage.Redis;

namespace Core.Cache.Storage;

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
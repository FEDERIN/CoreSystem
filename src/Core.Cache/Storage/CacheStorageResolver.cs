using Core.Cache.Abstractions;
using Core.Cache.Options;
using Core.Cache.Storage.Memory;
using Core.Cache.Storage.Redis;

namespace Core.Cache.Storage;

internal sealed class CacheStorageResolver : ICacheStorageResolver
{
    public CacheStorageResolver(
        CacheOptions options,
        MemoryStorage memoryStorage,
        RedisStorage? redisStorage = null)
    {
        switch (options.DefaultProvider)
        {
            case CacheProviderType.Redis:

                if (redisStorage is null)
                {
                    throw new InvalidOperationException(
                        "Redis is configured as the default provider, but Redis is not registered.");
                }

                Primary = redisStorage;
                Fallback = memoryStorage;
                HasFallback = true;

                break;

            case CacheProviderType.Memory:
            default:

                Primary = memoryStorage;
                Fallback = memoryStorage;
                HasFallback = false;

                break;
        }
    }

    public ICacheStorage Primary { get; }

    public ICacheStorage Fallback { get; }

    public bool HasFallback { get; }
}
using Core.DistributedCache.Abstractions;

namespace Core.DistributedCache.Storage;

internal record CacheEntryWrapper<T>
{
    public required T Value { get; set; }
    public CacheProviderType Origin { get; set; }
}
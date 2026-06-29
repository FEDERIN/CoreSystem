namespace Core.DistributedCache.Abstractions;

internal record CacheEntryWrapper<T>
{
    public required T Value { get; set; }
    public CacheProviderType Origin { get; set; }
}
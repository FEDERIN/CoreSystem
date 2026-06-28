namespace Core.DistributedCache.Abstractions;

public class CacheEntryWrapper<T>
{
    public required T Value { get; set; }
    public CacheProviderType Origin { get; set; }
}
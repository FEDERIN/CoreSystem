using Core.Cache.Abstractions;

namespace Core.Cache.Storage;

internal record CacheEntryWrapper<T>
{
    public required T Value { get; set; }
    public CacheProviderType Origin { get; set; }
}
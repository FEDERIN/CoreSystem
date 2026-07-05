using Core.Cache.Abstractions;
using Core.Cache.Storage.Abstractions;

namespace Core.Cache.Storage;

internal record CacheEntryWrapper<T> : ICacheEntry
{
    public required T Value { get; set; }
    public CacheProviderType Origin { get; set; }
    public DateTimeOffset? AbsoluteExpiration { get; init; }
    object ICacheEntry.Value => Value!;
}
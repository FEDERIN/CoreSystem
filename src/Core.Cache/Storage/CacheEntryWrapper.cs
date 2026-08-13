using Core.Cache.Storage.Abstractions;

namespace Core.Cache.Storage;

internal record CacheEntryWrapper<T> : ICacheEntry
{
    public required T Value { get; set; }
    public DateTimeOffset? AbsoluteExpiration { get; init; }
    public IReadOnlyCollection<string>? Tags { get; init; }
    object ICacheEntry.Value => Value!;
}
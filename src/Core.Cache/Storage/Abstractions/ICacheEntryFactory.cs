using Core.Cache.Abstractions;

namespace Core.Cache.Storage.Abstractions;

internal interface ICacheEntryFactory
{
    CacheEntryWrapper<T> Create<T>(T value, CacheEntryOptions options, DateTimeOffset? absoluteExpiration);

    bool TryUnwrap<T>(object? entry, out T? value);

    bool TryGetOrigin(object? entry, out CacheProviderType origin);

    bool TryGetValue(object? entry, out object? value);
}
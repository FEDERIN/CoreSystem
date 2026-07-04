using Core.Cache.Abstractions;

namespace Core.Cache.Storage.Abstractions;

internal interface ICacheEntryFactory
{
    CacheEntryWrapper<T> Create<T>(T value, CacheEntryOptions options);

    bool TryUnwrap<T>(
    object? entry,
    out T? value);

    bool TryGetOrigin(
    object? entry,
    out CacheProviderType origin);
}
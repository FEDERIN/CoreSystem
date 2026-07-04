using Core.Cache.Abstractions;

namespace Core.Cache.Storage.Abstractions;

internal interface ICacheEntryFactory
{
    CacheEntryWrapper<T> Create<T>(T value, CacheProviderType origin);

    bool TryUnwrap<T>(
    object? entry,
    out T? value);
}
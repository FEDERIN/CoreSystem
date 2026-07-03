using Core.Cache.Storage;

namespace Core.Cache.Storage.Abstractions;

internal interface ICacheEntryFactory
{
    CacheEntryWrapper<T> Create<T>(T value);

    bool TryUnwrap<T>(
    object? entry,
    out T? value);
}
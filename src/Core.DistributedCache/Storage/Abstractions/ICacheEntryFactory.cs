namespace Core.DistributedCache.Storage.Abstractions;

internal interface ICacheEntryFactory
{
    CacheEntryWrapper<T> Create<T>(T value);

    bool TryUnwrap<T>(
    object? entry,
    out T? value);
}
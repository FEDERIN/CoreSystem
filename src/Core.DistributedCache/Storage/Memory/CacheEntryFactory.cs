using Core.DistributedCache.Abstractions;
using Core.DistributedCache.Storage.Abstractions;

namespace Core.DistributedCache.Storage.Memory;

internal sealed class CacheEntryFactory : ICacheEntryFactory
{
    public CacheEntryWrapper<T> Create<T>(T value)
    {
        if (value is CacheEntryWrapper<T> wrapper)
        {
            return wrapper;
        }

        return new CacheEntryWrapper<T>
        {
            Value = value,
            Origin = CacheProviderType.Memory
        };
    }

    public bool TryUnwrap<T>(
    object? entry,
    out T? value)
    {
        if (entry is CacheEntryWrapper<T> wrapper)
        {
            value = wrapper.Value;
            return true;
        }

        value = default;
        return false;
    }
}
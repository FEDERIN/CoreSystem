using Core.Cache.Storage.Abstractions;

namespace Core.Cache.Storage.Memory;

internal sealed class CacheEntryInspector
    : ICacheEntryInspector
{
    public bool TryGet(
        object? entry,
        out ICacheEntry? wrapper)
    {
        wrapper = entry as ICacheEntry;

        return wrapper is not null;
    }

    public bool TryGetValue<T>(
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

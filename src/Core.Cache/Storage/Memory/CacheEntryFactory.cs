using Core.Cache.Abstractions;
using Core.Cache.Storage.Abstractions;

namespace Core.Cache.Storage.Memory;

internal sealed class CacheEntryFactory : ICacheEntryFactory
{
    public CacheEntryWrapper<T> Create<T>(T value, CacheEntryOptions options)
    {
        if (value is CacheEntryWrapper<T> wrapper)
        {
            return wrapper;
        }

        var origin = options.TrackForRehydration
            ? CacheProviderType.Redis
            : CacheProviderType.Memory;

        return new CacheEntryWrapper<T>
        {
            Value = value,
            Origin = origin
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

    public bool TryGetOrigin(
    object? entry,
    out CacheProviderType origin)
    {
        if (entry is null)
        {
            origin = default;
            return false;
        }

        var type = entry.GetType();

        if (!type.IsGenericType ||
            type.GetGenericTypeDefinition() != typeof(CacheEntryWrapper<>))
        {
            origin = default;
            return false;
        }

        origin = (CacheProviderType)type
            .GetProperty(nameof(CacheEntryWrapper<object>.Origin))!
            .GetValue(entry)!;

        return true;
    }
}
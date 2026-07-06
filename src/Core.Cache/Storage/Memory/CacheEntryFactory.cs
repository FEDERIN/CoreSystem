using Core.Cache.Abstractions;
using Core.Cache.Storage.Abstractions;

namespace Core.Cache.Storage.Memory;

internal sealed class CacheEntryFactory : ICacheEntryFactory
{
    public CacheEntryWrapper<T> Create<T>(T value, CacheEntryOptions options, DateTimeOffset? absoluteExpiration)
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
            Origin = origin,
            AbsoluteExpiration = absoluteExpiration
        };
    }
}
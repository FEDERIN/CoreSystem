using Core.Cache.Storage.Abstractions;

namespace Core.Cache.Storage.Memory;

internal sealed class CacheEntryFactory : ICacheEntryFactory
{
    public CacheEntryWrapper<T> Create<T>(T value, CacheEntryOptions options, DateTimeOffset? absoluteExpiration,
    IReadOnlyCollection<string>? tags)
    {
        if (value is CacheEntryWrapper<T> wrapper)
        {
            return wrapper;
        }

        return new CacheEntryWrapper<T>
        {
            Value = value,
            AbsoluteExpiration = absoluteExpiration,
            Tags = tags
        };
    }
}
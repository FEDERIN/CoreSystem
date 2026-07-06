using Core.Cache.Abstractions;

namespace Core.Cache.Storage.Abstractions;

internal interface ICacheEntryFactory
{
    CacheEntryWrapper<T> Create<T>(T value, CacheEntryOptions options, DateTimeOffset? absoluteExpiration);
}
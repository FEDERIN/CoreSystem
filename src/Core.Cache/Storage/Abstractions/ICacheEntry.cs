using Core.Cache.Abstractions;

namespace Core.Cache.Storage.Abstractions;

internal interface ICacheEntry
{
    object Value { get; }
    CacheProviderType Origin { get; }
    DateTimeOffset? AbsoluteExpiration { get; }
}
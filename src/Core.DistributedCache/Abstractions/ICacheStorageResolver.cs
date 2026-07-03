namespace Core.DistributedCache.Abstractions;

internal interface ICacheStorageResolver
{
    ICacheStorage Primary { get; }

    ICacheStorage Fallback { get; }
}
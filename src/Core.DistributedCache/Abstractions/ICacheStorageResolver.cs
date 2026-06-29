namespace Core.DistributedCache.Abstractions;

public interface ICacheStorageResolver
{
    ICacheStorage Primary { get; }

    ICacheStorage Fallback { get; }
}
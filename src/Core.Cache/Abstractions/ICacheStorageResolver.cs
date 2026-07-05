namespace Core.Cache.Abstractions;

internal interface ICacheStorageResolver
{
    ICacheStorage Primary { get; }

    ICacheStorage Fallback { get; }

    bool HasFallback { get; }
}
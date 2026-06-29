namespace Core.DistributedCache.Abstractions;

/// <summary>
/// Defines a delegate that represents the next step in the cache pipeline.
/// </summary>
public delegate Task CacheDelegate(CacheContext context);
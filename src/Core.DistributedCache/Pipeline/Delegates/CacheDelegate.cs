using Core.DistributedCache.Pipeline.Contexts;

namespace Core.DistributedCache.Pipeline.Delegates;

/// <summary>
/// Defines a delegate that represents the next step in the cache pipeline.
/// </summary>
public delegate Task CacheDelegate(CacheContext context);
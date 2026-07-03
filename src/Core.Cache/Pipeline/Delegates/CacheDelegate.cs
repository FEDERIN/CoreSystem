using Core.Cache.Pipeline.Contexts;

namespace Core.Cache.Pipeline.Delegates;

/// <summary>
/// Defines a delegate that represents the next step in the cache pipeline.
/// </summary>
public delegate Task CacheDelegate(CacheContext context);
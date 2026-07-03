using Core.DistributedCache.Pipeline.Contexts;
using Core.DistributedCache.Pipeline.Delegates;

namespace Core.DistributedCache.Pipeline.Abstractions;

/// <summary>
/// Defines a behavior that can be used to intercept and process cache operations.
/// </summary>
public interface ICacheBehavior
{
    /// <summary>
    /// Invokes the behavior in the pipeline.
    /// </summary>
    /// <param name="context">The context of the current cache operation.</param>
    /// <param name="next">The next delegate in the pipeline to be invoked.</param>
    Task InvokeAsync(
        CacheContext context,
        CacheDelegate next);
}
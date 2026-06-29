using Core.DistributedCache.Abstractions;

namespace Core.DistributedCache.Pipeline;

internal sealed class CachePipeline(IEnumerable<ICacheBehavior> behaviors) : ICachePipeline
{
    private readonly IReadOnlyList<ICacheBehavior> _behaviors = behaviors.ToList();

    public Task ExecuteAsync(
        CacheContext context,
        CacheDelegate terminal)
    {
        CacheDelegate next = terminal;

        for (int i = _behaviors.Count - 1; i >= 0; i--)
        {
            var behavior = _behaviors[i];
            var current = next;

            next = ctx => behavior.InvokeAsync(ctx, current);
        }

        return next(context);
    }
}
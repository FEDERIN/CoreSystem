using Core.Cache.Pipeline.Abstractions;
using Core.Cache.Pipeline.Contexts;
using Core.Cache.Pipeline.Delegates;

namespace Core.Cache.Pipeline;

internal sealed class CachePipeline(IEnumerable<ICacheBehavior> behaviors) : ICachePipeline
{
    private readonly IReadOnlyList<ICacheBehavior> _behaviors = [..behaviors];

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
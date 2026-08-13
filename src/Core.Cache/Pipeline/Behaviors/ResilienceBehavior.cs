using Core.Cache.Pipeline.Abstractions;
using Core.Cache.Pipeline.Contexts;
using Core.Cache.Pipeline.Delegates;
using Core.Resilience.Abstractions;

namespace Core.Cache.Pipeline.Behaviors;

internal sealed class ResilienceBehavior(
    IResiliencePipeline pipeline)
    : ICacheBehavior
{
    private readonly IResiliencePipeline _pipeline = pipeline;

    public Task InvokeAsync(
        CacheContext context,
        CacheDelegate next)
    {
        return _pipeline.ExecuteAsync(
            _ => next(context),
            context.CancellationToken);
    }
}
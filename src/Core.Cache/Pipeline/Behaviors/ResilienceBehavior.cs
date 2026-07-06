using Core.Cache.Pipeline.Abstractions;
using Core.Cache.Pipeline.Contexts;
using Core.Cache.Pipeline.Delegates;
using Polly;

namespace Core.Cache.Pipeline.Behaviors;

internal sealed class ResilienceBehavior(
    ResiliencePipeline pipeline)
    : ICacheBehavior
{
    private readonly ResiliencePipeline _pipeline = pipeline;

    public async Task InvokeAsync(CacheContext context, CacheDelegate next)
    {
        await _pipeline.ExecuteAsync(
            async token =>
            {
                await next(context);
            },
            context.CancellationToken);
    }
}
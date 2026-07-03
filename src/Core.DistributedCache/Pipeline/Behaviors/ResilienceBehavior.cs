using Core.DistributedCache.Abstractions;
using Core.DistributedCache.Pipeline.Abstractions;
using Core.DistributedCache.Pipeline.Contexts;
using Core.DistributedCache.Pipeline.Delegates;
using Polly;

namespace Core.DistributedCache.Pipeline.Behaviors;

internal sealed class ResilienceBehavior(
    ResiliencePipeline pipeline,
    IRedisHealthState healthState)
    : ICacheBehavior
{
    private readonly ResiliencePipeline _pipeline = pipeline;
    private readonly IRedisHealthState _healthState = healthState;

    public async Task InvokeAsync(CacheContext context, CacheDelegate next)
    {
        try
        {
            await _pipeline.ExecuteAsync(
                async token =>
                {
                    await next(context);
                },
                context.CancellationToken);

            _healthState.MarkHealthy();
        }
        catch
        {
            _healthState.MarkUnhealthy();
            throw;
        }
    }
}
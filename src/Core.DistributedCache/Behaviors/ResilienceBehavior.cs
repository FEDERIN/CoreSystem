using Core.DistributedCache.Abstractions;
using Polly;

namespace Core.DistributedCache.Behaviors;

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
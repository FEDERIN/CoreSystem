using Core.Cache.Abstractions;
using Core.Cache.Options;
using Core.Cache.Pipeline.Abstractions;
using Core.Cache.Pipeline.Contexts;
using Core.Cache.Pipeline.Delegates;
using Polly;

namespace Core.Cache.Pipeline.Behaviors;

internal sealed class ResilienceBehavior(
    ResiliencePipeline pipeline,
    CacheOptions options,
    IHealthState healthState)
    : ICacheBehavior
{
    private readonly ResiliencePipeline _pipeline = pipeline;
    private readonly IHealthState _healthState = healthState;

    public async Task InvokeAsync(CacheContext context, CacheDelegate next)
    {
        try
        {
            // Skip the resilience pipeline for the in-memory provider.
            // Memory operations are local and do not require retries or circuit breakers.
            if (options.DefaultProvider == CacheProviderType.Memory)
            {
                await next(context);
                return;
            }

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
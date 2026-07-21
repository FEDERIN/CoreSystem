using Core.Resilience.Diagnostics;
using Core.Resilience.Internal.Constants;
using Core.Resilience.Internal.Extensions;
using Core.Resilience.Internal.Helpers;
using Core.Resilience.Internal.Strategies;
using Core.Resilience.Options;
using Polly;
using Polly.Retry;
using System.Diagnostics;

internal sealed class RetryStrategyBuilder(
    ResilienceMetrics metrics) : IStrategyBuilder
{
    private readonly ResilienceMetrics _metrics = metrics;

    public int Order => StrategyOrder.Retry;

    public void Configure(
        ResiliencePipelineBuilder builder,
        PipelineOptions options)
    {
        if (options.Retry is not { Enabled: true } retryOptions)
        {
            return;
        }

        builder.AddRetry(CreateRetryOptions(retryOptions));
    }

    private RetryStrategyOptions CreateRetryOptions(
        RetryOptions options)
    {
        var retry = new RetryStrategyOptions
        {
            MaxRetryAttempts = options.MaxRetryAttempts,
            Delay = options.Delay,
            BackoffType = options.BackoffType.ToPolly(),
            UseJitter = options.UseJitter,

            OnRetry = args =>
            {
                TagList tags = new()
                {
                    { "strategy", "retry" },
                    { "attempt", args.AttemptNumber }
                };

                _metrics.RecordRetry(tags);
                return default;
            }
        };

        if (options.HandledExceptions.Count > 0)
        {
            retry.ShouldHandle =
                PredicateBuilderFactory.Create(options.HandledExceptions);
        }

        return retry;
    }
}
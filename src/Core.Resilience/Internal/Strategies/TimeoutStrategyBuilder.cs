using Core.Resilience.Diagnostics;
using Core.Resilience.Internal.Constants;
using Core.Resilience.Internal.Strategies;
using Core.Resilience.Options;
using Polly;
using Polly.Timeout;
using System.Diagnostics;

internal sealed class TimeoutStrategyBuilder(
    ResilienceMetrics metrics) : IStrategyBuilder
{
    private readonly ResilienceMetrics _metrics = metrics;

    public int Order => StrategyOrder.Timeout;

    public void Configure(
        ResiliencePipelineBuilder builder,
        PipelineOptions options)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(options);

        if (options.Timeout is null)
        {
            return;
        }

        builder.AddTimeout(CreateOptions(options.Timeout));
    }

    private TimeoutStrategyOptions CreateOptions(
        TimeoutOptions options)
    {
        return new TimeoutStrategyOptions
        {
            Timeout = options.Timeout,

            OnTimeout = args =>
            {
                TagList tags = new()
                {
                    { "strategy", "timeout" }
                };

                _metrics.RecordTimeout(tags);
                return default;
            }
        };
    }
}
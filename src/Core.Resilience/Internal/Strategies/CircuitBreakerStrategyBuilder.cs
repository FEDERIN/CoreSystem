using Core.Resilience.Diagnostics;
using Core.Resilience.Internal.Constants;
using Core.Resilience.Internal.Helpers;
using Core.Resilience.Options;
using Polly;
using Polly.CircuitBreaker;

namespace Core.Resilience.Internal.Strategies;

internal sealed class CircuitBreakerStrategyBuilder(ResilienceMetrics metrics) 
    : IStrategyBuilder
{
    private readonly ResilienceMetrics _metrics = metrics;

    public int Order => StrategyOrder.CircuitBreaker;

    public void Configure(
        ResiliencePipelineBuilder builder,
        PipelineOptions options)
    {
        if (options.CircuitBreaker is not { Enabled: true } circuitBreaker)
        {
            return;
        }

        builder.AddCircuitBreaker(CreateOptions(circuitBreaker));
    }

    private CircuitBreakerStrategyOptions CreateOptions(
        CircuitBreakerOptions options)
    {
        var circuit = new CircuitBreakerStrategyOptions
        {
            FailureRatio = options.FailureRatio,
            MinimumThroughput = options.MinimumThroughput,
            SamplingDuration = options.SamplingDuration,
            BreakDuration = options.BreakDuration,

            OnOpened = args =>
            {
                _metrics.RecordCircuitOpened();
                return default;
            },

            OnClosed = args =>
            {
                _metrics.RecordCircuitClosed();
                return default;
            },

            OnHalfOpened = args =>
            {
                _metrics.RecordCircuitHalfOpened();

                return default;
            }
        };

        if (options.HandledExceptions.Count > 0)
        {
            circuit.ShouldHandle =
                PredicateBuilderFactory.Create(options.HandledExceptions);
        }

        return circuit;
    }
}
namespace Core.Resilience.Options;

/// <summary>
/// Represents the resilience strategies configured for a pipeline.
/// Only configured strategies are added to the execution pipeline.
/// </summary>
public sealed class PipelineOptions
{
    /// <summary>
    /// Configures the retry strategy.
    /// </summary>
    public RetryOptions? Retry { get; set; }

    /// <summary>
    /// Configures the circuit breaker strategy.
    /// </summary>
    public CircuitBreakerOptions? CircuitBreaker { get; set; }

    /// <summary>
    /// Configures the timeout strategy.
    /// </summary>
    public TimeoutOptions? Timeout { get; set; }
}
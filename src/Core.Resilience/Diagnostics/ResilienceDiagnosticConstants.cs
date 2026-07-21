namespace Core.Resilience.Diagnostics;

internal static class ResilienceDiagnosticConstants
{
    public const string ActivitySourceName = "Core.Resilience";
    public const string MeterName = "Core.Resilience";
    public const string Version = "1.0.0";

    public const string ExecutionActivityName = "resilience.execute";

    public const string PipelineExecutionMetric =
        "core.resilience.pipeline.executions";
    public const string PipelineExecutionDescription =
        "Total number of executions processed by the resilience pipeline.";

    public const string PipelineFailureMetric =
        "core.resilience.pipeline.failures";
    public const string PipelineFailureDescription =
        "Total number of executions that completed with an exception after all configured resilience strategies were applied.";

    public const string ExecutionDurationMetric =
        "core.resilience.pipeline.duration";
    public const string ExecutionDurationDescription =
        "Execution time of the resilience pipeline, including retries, timeouts, and other configured resilience strategies.";

    public const string RetryAttemptsMetric =
        "core.resilience.retry.attempts";
    public const string RetryAttemptsDescription =
        "Total number of retry attempts executed by the retry strategy.";

    public const string TimeoutTriggeredMetric =
        "core.resilience.timeout.triggered";
    public const string TimeoutTriggeredDescription =
        "Total number of operations that exceeded the configured timeout.";

    public const string CircuitOpenedMetric =
        "core.resilience.circuit.opened";
    public const string CircuitOpenedDescription =
        "Total number of times the circuit breaker transitioned to the Open state.";

    public const string CircuitHalfOpenedMetric =
        "core.resilience.circuit.half_opened";
    public const string CircuitHalfOpenedDescription =
        "Total number of times the circuit breaker transitioned to the Half-Open state.";

    public const string CircuitClosedMetric =
        "core.resilience.circuit.closed";
    public const string CircuitClosedDescription =
        "Total number of times the circuit breaker transitioned to the Closed state.";
}
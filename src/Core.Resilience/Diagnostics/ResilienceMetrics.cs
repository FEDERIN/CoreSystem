using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Core.Resilience.Diagnostics;

internal sealed class ResilienceMetrics
{
    private readonly Counter<long> _retryAttempts;
    private readonly Counter<long> _timeouts;
    private readonly Counter<long> _circuitOpened;
    private readonly Counter<long> _circuitClosed;
    private readonly Counter<long> _circuitHalfOpened;

    private readonly Histogram<double> _executionDuration;

    public ResilienceMetrics(IMeterFactory meterFactory)
    {
        ArgumentNullException.ThrowIfNull(meterFactory);

        var meter = meterFactory.Create(
            ResilienceDiagnosticConstants.MeterName,
            ResilienceDiagnosticConstants.Version);

        _retryAttempts = meter.CreateCounter<long>(
            name: ResilienceDiagnosticConstants.RetryAttemptsMetric,
            unit: "{attempts}",
            description: ResilienceDiagnosticConstants.RetryAttemptsDescription);

        _timeouts = meter.CreateCounter<long>(
            name: ResilienceDiagnosticConstants.TimeoutTriggeredMetric,
            unit: "{timeouts}",
            description: ResilienceDiagnosticConstants.TimeoutTriggeredDescription);

        _circuitOpened = meter.CreateCounter<long>(
            name: ResilienceDiagnosticConstants.CircuitOpenedMetric,
            unit: "{events}",
            description: ResilienceDiagnosticConstants.CircuitOpenedDescription);

        _circuitClosed = meter.CreateCounter<long>(
            name: ResilienceDiagnosticConstants.CircuitClosedMetric,
            unit: "{events}",
            description: ResilienceDiagnosticConstants.CircuitClosedDescription);

        _circuitHalfOpened = meter.CreateCounter<long>(
            name: ResilienceDiagnosticConstants.CircuitHalfOpenedMetric,
            unit: "{events}",
            description: ResilienceDiagnosticConstants.CircuitHalfOpenedDescription);

        _executionDuration = meter.CreateHistogram<double>(
            name: ResilienceDiagnosticConstants.ExecutionDurationMetric,
            unit: "ms",
            description: ResilienceDiagnosticConstants.ExecutionDurationDescription);
    }

    public void RecordRetry(in TagList tags)
        => _retryAttempts.Add(1, tags);

    public void RecordTimeout(in TagList tags)
        => _timeouts.Add(1, tags);

    public void RecordCircuitOpened()
        => _circuitOpened.Add(1);

    public void RecordCircuitClosed()
        => _circuitClosed.Add(1);

    public void RecordCircuitHalfOpened()
        => _circuitHalfOpened.Add(1);


    public void RecordExecutionDuration(
        double milliseconds,
        in TagList tags)
        => _executionDuration.Record(milliseconds, tags);
}
using Core.Resilience.Diagnostics;
using Core.Resilience.UnitTests.Internal.Strategies;
using FluentAssertions;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Core.Resilience.UnitTests.Diagnostics;

public sealed class ResilienceMetricsTests
{
    [Fact]
    public void RecordExecutionDuration_ShouldRecordDuration()
    {
        // Arrange
        using var listener = new MeterListener();

        double? recordedValue = null;

        listener.InstrumentPublished = (instrument, listener) =>
        {
            if (instrument.Name ==
                "core.resilience.pipeline.duration")
            {
                listener.EnableMeasurementEvents(instrument);
            }
        };

        listener.SetMeasurementEventCallback<double>(
            (instrument, value, tags, state) =>
            {
                recordedValue = value;
            });

        listener.Start();

        var meterFactory = new TestMeterFactory();

        var metrics = new ResilienceMetrics(meterFactory);

        TagList tags = new()
        {
            { "strategy", "test" }
        };

        // Act
        metrics.RecordExecutionDuration(
            125.5,
            tags);

        // Assert
        recordedValue.Should().Be(125.5);
    }
}
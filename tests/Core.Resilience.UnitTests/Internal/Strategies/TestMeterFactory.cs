using System.Diagnostics.Metrics;

namespace Core.Resilience.UnitTests.Internal.Strategies;

internal sealed class TestMeterFactory : IMeterFactory
{
    public Meter Create(MeterOptions options)
        => new(options.Name, options.Version);

    public void Dispose()
    {
    }
}
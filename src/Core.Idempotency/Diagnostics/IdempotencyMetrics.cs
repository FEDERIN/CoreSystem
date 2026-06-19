using System.Diagnostics.Metrics;

namespace Core.Idempotency.Diagnostics;

/// <summary>
/// Provides telemetry metrics for the idempotency system.
/// </summary>
public class IdempotencyMetrics
{
    private readonly Counter<long>? _cacheHitCounter;
    private readonly Counter<long>? _cacheMissCounter;

    public IdempotencyMetrics(IMeterFactory meterFactory, string? meterName)
    {
        if (string.IsNullOrWhiteSpace(meterName)) return;

        var meter = meterFactory.Create(meterName, "1.0.0");

        _cacheHitCounter = meter.CreateCounter<long>(
            name: "idempotency.cache.hits",
            unit: "{hits}",
            description: "Total requests served from idempotency storage (Cache Hit)");

        _cacheMissCounter = meter.CreateCounter<long>(
            name: "idempotency.cache.misses",
            unit: "{misses}",
            description: "Total new requests processed (Cache Miss)");
    }

    // Use null-conditional operator to avoid NullReferenceException
    public void RecordHit() => _cacheHitCounter?.Add(1);
    public void RecordMiss() => _cacheMissCounter?.Add(1);
}
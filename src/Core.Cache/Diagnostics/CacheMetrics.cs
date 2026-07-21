using System.Diagnostics.Metrics;

namespace Core.Cache.Diagnostics;

public class CacheMetrics
{
    private readonly Counter<long>? _cacheHitCounter;
    private readonly Counter<long>? _cacheMissCounter;

    public CacheMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(CacheDiagnosticsConstants.MeterName!, "1.0.0");

        _cacheHitCounter = meter.CreateCounter<long>(
            name: "cache.distributed.hits",
            unit: "{hits}",
            description: "Total number of successful cache reads.");

        _cacheMissCounter = meter.CreateCounter<long>(
            name: "cache.distributed.misses",
            unit: "{misses}",
            description: "Total number of cache read requests that did not find the requested item.");
    }

    public void RecordHit() => _cacheHitCounter?.Add(1);

    public void RecordMiss() => _cacheMissCounter?.Add(1);
}
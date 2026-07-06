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
            description: "Número total de lecturas exitosas desde el caché.");

        _cacheMissCounter = meter.CreateCounter<long>(
            name: "cache.distributed.misses",
            unit: "{misses}",
            description: "Número total de lecturas que no encontraron el elemento (Cache Miss).");
    }

    public void RecordHit() => _cacheHitCounter?.Add(1);
    public void RecordMiss() => _cacheMissCounter?.Add(1);
}
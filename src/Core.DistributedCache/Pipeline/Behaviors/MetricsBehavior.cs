using Core.DistributedCache.Diagnostics;
using Core.DistributedCache.Pipeline.Contexts;
using Core.DistributedCache.Pipeline.Delegates;
using Core.DistributedCache.Pipeline.Interfaces;

namespace Core.DistributedCache.Pipeline.Behaviors;

internal sealed class MetricsBehavior(
    CacheMetrics metrics)
    : ICacheBehavior
{
    public async Task InvokeAsync(
        CacheContext context,
        CacheDelegate next)
    {
        await next(context);

        if (context is not ICacheMetricContext metric)
            return;

        switch (metric.MetricKind)
        {
            case CacheMetricKind.Hit:
                metrics.RecordHit();
                break;

            case CacheMetricKind.Miss:
                metrics.RecordMiss();
                break;
        }
    }
}
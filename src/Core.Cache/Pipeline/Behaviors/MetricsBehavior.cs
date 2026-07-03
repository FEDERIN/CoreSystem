using Core.Cache.Diagnostics;
using Core.Cache.Pipeline.Abstractions;
using Core.Cache.Pipeline.Contexts;
using Core.Cache.Pipeline.Delegates;

namespace Core.Cache.Pipeline.Behaviors;

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
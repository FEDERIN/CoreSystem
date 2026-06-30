using Core.DistributedCache.Abstractions;

namespace Core.DistributedCache.Contexts;

public sealed class GetCacheContext<T> : CacheContext, ICacheMetricContext
{
    public T? Result { get; set; }
    public CacheMetricKind MetricKind =>
        Result is null
            ? CacheMetricKind.Miss
            : CacheMetricKind.Hit;

    public override async Task ExecuteAsync()
    {
        Result = await Storage.GetAsync<T>(
            Key,
            CancellationToken);
    }
}
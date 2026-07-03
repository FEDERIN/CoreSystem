using Core.DistributedCache.Pipeline.Abstractions;

namespace Core.DistributedCache.Pipeline.Contexts;

public sealed class GetOrAddCacheContext<T> : CacheContext, ICacheMetricContext
{
    public required Func<CancellationToken, Task<T>> Factory { get; init; }
    public TimeSpan? Expiration { get; init; }
    public string[]? Tags { get; init; }
    public T? Result { get; set; }
    public CacheMetricKind MetricKind =>
        Result is null
            ? CacheMetricKind.Miss
            : CacheMetricKind.Hit;

    public override async Task ExecuteAsync()
    {
        Result = await Storage.GetOrAddAsync(
            Key,
            Factory,
            Expiration,
            Tags,
            CancellationToken);
    }
}
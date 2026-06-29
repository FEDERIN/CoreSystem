namespace Core.DistributedCache.Abstractions;

public interface ICacheMetricContext
{
    CacheMetricKind MetricKind { get; }
}

public enum CacheMetricKind
{
    None,
    Hit,
    Miss
}
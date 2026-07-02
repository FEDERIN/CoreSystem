namespace Core.DistributedCache.Pipeline.Interfaces;

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
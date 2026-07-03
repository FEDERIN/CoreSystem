namespace Core.Cache.Pipeline.Abstractions;

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
namespace Core.DistributedCache.Abstractions;

public interface IRedisHealthState
{
    bool IsRedisHealthy { get; }
    void MarkHealthy();
    void MarkUnhealthy();
}
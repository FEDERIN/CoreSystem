namespace Core.Cache.Abstractions;

public interface IRedisHealthState
{
    bool IsRedisHealthy { get; }
    void MarkHealthy();
    void MarkUnhealthy();
}
namespace Core.Cache.Abstractions;

public interface IHealthState
{
    bool IsRedisHealthy { get; }
    void MarkHealthy();
    void MarkUnhealthy();
}
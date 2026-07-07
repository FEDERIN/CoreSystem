namespace Core.Redis.Options;

public sealed class RedisLockOptions
{
    public TimeSpan LockDuration { get; set; } = TimeSpan.FromSeconds(30);

    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromMilliseconds(100);

    public TimeSpan? MaxWaitTime { get; set; }
}
namespace Core.Redis.Options;

/// <summary>
/// Options that control the behavior of Redis distributed locks.
/// </summary>
public sealed class RedisLockOptions
{
    /// <summary>
    /// Gets or sets the duration for which the lock is held.
    /// </summary>
    public TimeSpan LockDuration { get; set; } =
        TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets the delay between retry attempts.
    /// </summary>
    public TimeSpan RetryDelay { get; set; } =
        TimeSpan.FromMilliseconds(100);

    /// <summary>
    /// Gets or sets the maximum time to wait for a lock.
    /// If null, retries indefinitely.
    /// </summary>
    public TimeSpan? MaxWaitTime { get; set; }
    public Action<object> Configuration { get; set; }
}
namespace Core.Redis.Synchronization;

/// <summary>
/// Provides distributed synchronization using Redis-based locks.
/// </summary>
public interface IDistributedLockProvider
{
    /// <summary>
    /// Acquires an exclusive distributed lock for the specified key.
    /// </summary>
    /// <param name="lockKey">
    /// Unique lock identifier.
    /// </param>
    /// <param name="ct">
    /// Cancellation token.
    /// </param>
    /// <returns>
    /// A disposable handle that releases the lock when disposed.
    /// </returns>
    Task<IDisposable> AcquireAsync(
        string lockKey,
        CancellationToken ct = default);
}
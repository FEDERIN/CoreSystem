namespace Core.Memory.Synchronization;

/// <summary>
/// Provides asynchronous keyed locks for coordinating concurrent
/// operations within a single process.
/// </summary>
/// <remarks>
/// Each unique key represents an independent asynchronous lock,
/// allowing unrelated operations to execute concurrently while
/// serializing access to the same logical resource.
///
/// This abstraction is intended for in-process synchronization only
/// and does not provide distributed locking across multiple application
/// instances or servers.
///
/// Typical scenarios include:
/// <list type="bullet">
/// <item>
/// <description>Preventing concurrent processing of the same entity.</description>
/// </item>
/// <item>
/// <description>Synchronizing access to shared in-memory resources.</description>
/// </item>
/// <item>
/// <description>Avoiding duplicate execution of background jobs.</description>
/// </item>
/// </list>
/// </remarks>
public interface IAsyncKeyLock
{
    /// <summary>
    /// Acquires an asynchronous lock associated with the specified key.
    /// </summary>
    /// <param name="key">
    /// A logical identifier that uniquely represents the resource to
    /// synchronize.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the asynchronous wait operation.
    /// </param>
    /// <returns>
    /// A disposable lock handle that releases the lock when disposed.
    /// </returns>
    /// <remarks>
    /// Operations using the same key are executed sequentially,
    /// while operations using different keys may execute concurrently.
    ///
    /// The returned handle should be disposed using
    /// <c>await using</c> to ensure the lock is released.
    /// </remarks>
    Task<IDisposable> AcquireAsync(
        string key,
        CancellationToken cancellationToken = default);
}
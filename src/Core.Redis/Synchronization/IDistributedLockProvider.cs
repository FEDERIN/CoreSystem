namespace Core.Redis.Synchronization;

public interface IDistributedLockProvider
{
    Task<IDisposable> AcquireAsync(
        string lockKey,
        CancellationToken ct = default);
}
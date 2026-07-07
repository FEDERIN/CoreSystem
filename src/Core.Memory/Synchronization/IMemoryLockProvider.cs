namespace Core.Memory.Synchronization;

public interface IMemoryLockProvider
{
    Task<IDisposable> AcquireAsync(
        string key,
        CancellationToken cancellationToken = default);
}
namespace Core.DistributedCache.Storage.Abstractions;

public interface ICacheLockProvider
{
    Task<IDisposable> AcquireAsync(string key, CancellationToken ct = default);
}
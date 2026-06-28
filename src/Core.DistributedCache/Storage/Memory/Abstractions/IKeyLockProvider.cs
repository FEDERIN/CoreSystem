namespace Core.DistributedCache.Storage.Memory.Abstractions;

public interface IKeyLockProvider
{
    Task<IDisposable> AcquireAsync(string key, CancellationToken ct = default);
}
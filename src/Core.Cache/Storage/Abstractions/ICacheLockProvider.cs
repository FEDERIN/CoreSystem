namespace Core.Cache.Storage.Abstractions;

public interface ICacheLockProvider<TProvider>
{
    Task<IDisposable> AcquireAsync(string key, CancellationToken ct = default);
}
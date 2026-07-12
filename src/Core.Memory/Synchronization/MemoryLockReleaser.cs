namespace Core.Memory.Synchronization;

internal sealed class MemoryLockReleaser(
    string key,
    MemoryLockEntry entry,
    ILockRegistry registry)
    : IDisposable
{
    private bool _disposed;

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        entry.Semaphore.Release();

        registry.Release(
            key,
            entry);
    }
}
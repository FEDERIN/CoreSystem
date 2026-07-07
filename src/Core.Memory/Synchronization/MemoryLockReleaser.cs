using System.Collections.Concurrent;

namespace Core.Memory.Synchronization;

internal sealed class MemoryLockReleaser(
    string key,
    MemoryLockEntry entry,
    ConcurrentDictionary<string, MemoryLockEntry> locks)
    : IDisposable
{
    private bool _disposed;

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        entry.Semaphore.Release();

        if (entry.ReleaseReference() != 0)
            return;

        locks.TryRemove(
            new KeyValuePair<string, MemoryLockEntry>(
                key,
                entry));

        entry.Semaphore.Dispose();
    }
}
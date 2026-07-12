using Core.Memory.Synchronization;
using System.Collections.Concurrent;

internal sealed class MemoryLockRegistry : ILockRegistry
{
    private readonly ConcurrentDictionary<string, MemoryLockEntry> _locks = [];

    public MemoryLockEntry GetOrCreate(string key)
    {
        return _locks.GetOrAdd(
            key,
            static _ => new MemoryLockEntry());
    }

    public void Release(
        string key,
        MemoryLockEntry entry)
    {
        if (entry.ReleaseReference() != 0)
            return;

        _locks.TryRemove(
            new KeyValuePair<string, MemoryLockEntry>(
                key,
                entry));

        entry.Semaphore.Dispose();
    }
}
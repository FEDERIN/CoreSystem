using System.Collections.Concurrent;

namespace Core.Memory.Synchronization;

internal class MemoryLockProvider : IMemoryLockProvider
{
    private readonly ConcurrentDictionary<string, MemoryLockEntry> _locks = [];

    public async Task<IDisposable> AcquireAsync(
        string key,
        CancellationToken ct = default)
    {
        while (true)
        {
            ct.ThrowIfCancellationRequested();

            var entry = _locks.GetOrAdd(
                key,
                static _ => new MemoryLockEntry());

            entry.AddReference();

            try
            {
                await entry.Semaphore.WaitAsync(ct);

                return new MemoryLockReleaser(
                    key,
                    entry,
                    _locks);
            }
            catch
            {
                if (entry.ReleaseReference() == 0)
                {
                    _locks.TryRemove(
                        new KeyValuePair<string, MemoryLockEntry>(
                            key,
                            entry));

                    entry.Semaphore.Dispose();
                }

                throw;
            }
        }
    }
}

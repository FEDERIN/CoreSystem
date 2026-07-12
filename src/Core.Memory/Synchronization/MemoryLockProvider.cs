using Core.Memory.Synchronization;

internal sealed class MemoryLockProvider : IAsyncKeyLock
{
    private readonly ILockRegistry _registry;

    public MemoryLockProvider()
        : this(new MemoryLockRegistry())
    {
    }

    internal MemoryLockProvider(
        ILockRegistry registry)
    {
        _registry = registry;
    }

    public async Task<IDisposable> AcquireAsync(
        string key,
        CancellationToken ct = default)
    {
        while (true)
        {
            ct.ThrowIfCancellationRequested();

            var entry = _registry.GetOrCreate(key);

            entry.AddReference();

            try
            {
                await entry.Semaphore.WaitAsync(ct);

                return new MemoryLockReleaser(
                    key,
                    entry,
                    _registry);
            }
            catch
            {
                _registry.Release(key, entry);

                throw;
            }
        }
    }
}
namespace Core.Memory.Synchronization;

internal sealed class MemoryLockEntry
{
    public SemaphoreSlim Semaphore { get; } = new(1, 1);

    private int _referenceCount;

    public int AddReference()
        => Interlocked.Increment(ref _referenceCount);

    public int ReleaseReference()
        => Interlocked.Decrement(ref _referenceCount);
}
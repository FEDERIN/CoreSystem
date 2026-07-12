using Core.Memory.Synchronization;

internal interface ILockRegistry
{
    MemoryLockEntry GetOrCreate(string key);

    void Release(
        string key,
        MemoryLockEntry entry);
}
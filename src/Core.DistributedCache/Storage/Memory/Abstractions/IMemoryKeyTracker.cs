namespace Core.DistributedCache.Storage.Memory.Abstractions;

public interface IMemoryKeyTracker
{
    void Track(string key);
    void Untrack(string key);
    IEnumerable<string> GetAllTrackedKeys();
}
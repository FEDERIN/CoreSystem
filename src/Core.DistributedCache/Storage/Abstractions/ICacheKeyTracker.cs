namespace Core.DistributedCache.Storage.Abstractions;

public interface ICacheKeyTracker
{
    void Track(string key);
    void Untrack(string key);
    IEnumerable<string> GetAllTrackedKeys();
}
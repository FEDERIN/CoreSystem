namespace Core.Cache.Storage.Abstractions;

internal interface ICacheKeyTracker
{
    void Track(string key);
    void Untrack(string key);
    IEnumerable<string> GetAllTrackedKeys();
}
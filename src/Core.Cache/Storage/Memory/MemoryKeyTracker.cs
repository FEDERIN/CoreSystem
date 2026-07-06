using Core.Cache.Storage.Abstractions;
using System.Collections.Concurrent;

namespace Core.Cache.Storage.Memory;

public class MemoryKeyTracker : ICacheKeyTracker
{
    private readonly ConcurrentDictionary<string, byte> _trackedKeys = new();

    public void Track(string key) => _trackedKeys.TryAdd(key, 1);
    public void Untrack(string key) => _trackedKeys.TryRemove(key, out _);
    public IEnumerable<string> GetAllTrackedKeys() => _trackedKeys.Keys;
}
using Core.DistributedCache.Storage.Memory.Abstractions;
using System.Collections.Concurrent;

namespace Core.DistributedCache.Storage.Memory;

public class MemoryKeyTracker : IMemoryKeyTracker
{
    private readonly ConcurrentDictionary<string, byte> _trackedKeys = new();

    public void Track(string key) => _trackedKeys.TryAdd(key, 1);
    public void Untrack(string key) => _trackedKeys.TryRemove(key, out _);
    public IEnumerable<string> GetAllTrackedKeys() => _trackedKeys.Keys;
}
using Core.DistributedCache.Storage.Memory.Abstractions;
using System.Collections.Concurrent;

namespace Core.DistributedCache.Storage.Memory;

public class MemoryTagIndex : IMemoryTagIndex
{
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> _tagToKeys = new();
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> _keyToTags = new();

    public void AddTags(string key, string[] tags)
    {
        var keyTags = _keyToTags.GetOrAdd(key, _ => new());
        foreach (var tag in tags)
        {
            _tagToKeys.GetOrAdd(tag, _ => new())[key] = 1;
            keyTags[tag] = 1;
        }
    }

    public void RemoveKey(string key)
    {
        if (_keyToTags.TryRemove(key, out var tags))
        {
            foreach (var tag in tags.Keys)
            {
                if (_tagToKeys.TryGetValue(tag, out var keys))
                    keys.TryRemove(key, out _);
            }
        }
    }

    public void RemoveByTag(string tag, Action<string> onKeyRemoved)
    {
        if (_tagToKeys.TryRemove(tag, out var keys))
        {
            foreach (var key in keys.Keys)
            {
                onKeyRemoved(key);

                if (_keyToTags.TryGetValue(key, out var tags))
                {
                    tags.TryRemove(tag, out _);

                    if (tags.IsEmpty)
                    {
                        _keyToTags.TryRemove(key, out _);
                    }
                }
            }
        }
    }
}
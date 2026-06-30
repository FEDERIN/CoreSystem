using Core.DistributedCache.Storage.Abstractions;
using System.Collections.Concurrent;
using StringSet = System.Collections.Concurrent.ConcurrentDictionary<string, byte>;

namespace Core.DistributedCache.Storage.Memory;

internal sealed class MemoryTagIndex : ICacheTagIndex
{
    private readonly ConcurrentDictionary<string, StringSet> _tagKeys = new();
    private readonly ConcurrentDictionary<string, StringSet> _keyTags = new();

    public Task AddAsync(
        string key,
        IReadOnlyCollection<string> tags,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var keyTags = _keyTags.GetOrAdd(key, static _ => CreateSet());

        foreach (var tag in tags)
        {
            _tagKeys.GetOrAdd(tag, static _ => CreateSet())[key] = 0;
            keyTags[tag] = 0;
        }

        return Task.CompletedTask;
    }

    public async Task InvalidateTagAsync(
        string tag,
        Func<string, CancellationToken, Task> removeEntry,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_tagKeys.TryRemove(tag, out var keys))
        {
            return;
        }

        foreach (var key in keys.Keys)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await removeEntry(key.ToString(), cancellationToken);
            }
            finally
            {
                if (_keyTags.TryGetValue(key, out var tags))
                {
                    tags.TryRemove(tag, out _);

                    if (tags.IsEmpty)
                    {
                        _keyTags.TryRemove(key, out _);
                    }
                }
            }
        }
    }

    public Task RemoveKeyAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_keyTags.TryRemove(key, out var tags))
        {
            return Task.CompletedTask;
        }

        foreach (var tag in tags.Keys)
        {
            if (_tagKeys.TryGetValue(tag, out var keys))
            {
                keys.TryRemove(key, out _);

                if (keys.IsEmpty)
                {
                    _tagKeys.TryRemove(tag, out _);
                }
            }
        }

        return Task.CompletedTask;
    }

    private static ConcurrentDictionary<string, byte> CreateSet()
    => new();
}
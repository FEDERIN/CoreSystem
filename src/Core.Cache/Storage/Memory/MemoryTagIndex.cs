using Core.Cache.Storage.Abstractions;
using System.Collections.Concurrent;
using StringSet = System.Collections.Concurrent.ConcurrentDictionary<string, byte>;

namespace Core.Cache.Storage.Memory;

internal sealed class MemoryTagIndex : ICacheTagIndex<MemoryStorage>
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
            _tagKeys
                .GetOrAdd(tag, static _ => CreateSet())[key] = 0;

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
            return;

        var tasks = keys.Keys.Select(async key =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await removeEntry(key, cancellationToken);
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
        });

        await Task.WhenAll(tasks);
    }

    public Task RemoveKeyAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_keyTags.TryRemove(key, out var tags))
            return Task.CompletedTask;

        foreach (var tag in tags.Keys)
        {
            if (!_tagKeys.TryGetValue(tag, out var keys))
                continue;

            keys.TryRemove(key, out _);

            if (keys.IsEmpty)
            {
                _tagKeys.TryRemove(tag, out _);
            }
        }

        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<string>> GetKeysAsync(
        string tag,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_tagKeys.TryGetValue(tag, out var keys))
        {
            return Task.FromResult<IReadOnlyCollection<string>>(Array.Empty<string>());
        }

        return Task.FromResult<IReadOnlyCollection<string>>(keys.Keys.ToArray());
    }

    public Task<long> CountAsync(
        string tag,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        long count = _tagKeys.TryGetValue(tag, out var keys)
            ? keys.Count
            : 0;

        return Task.FromResult(count);
    }

    public Task<bool> ExistsAsync(
        string tag,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        bool exists =
            _tagKeys.TryGetValue(tag, out var keys) &&
            !keys.IsEmpty;

        return Task.FromResult(exists);
    }

    private static StringSet CreateSet() => new();
}
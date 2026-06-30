using Core.DistributedCache.Storage.Abstractions;
using StackExchange.Redis;

namespace Core.DistributedCache.Storage.Redis;

internal sealed class RedisTagIndex(
    IDatabase database,
    IKeyBuilder keyBuilder) : ICacheTagIndex
{
    private readonly IDatabase _database = database;
    private readonly IKeyBuilder _keyBuilder = keyBuilder;

    public async Task AddAsync(
        string key,
        IReadOnlyCollection<string> tags,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (tags.Count == 0)
        {
            return;
        }

        var batch = _database.CreateBatch();

        var tasks = new List<Task>(tags.Count * 2);

        foreach (var tag in tags)
        {
            tasks.Add(batch.SetAddAsync(
                _keyBuilder.BuildTag(tag),
                key));

            tasks.Add(batch.SetAddAsync(
                _keyBuilder.BuildTagsIndex(key),
                tag));
        }

        batch.Execute();

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    public async Task RemoveKeyAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var keyTagsKey = _keyBuilder.BuildTagsIndex(key);

        var tags = await _database
            .SetMembersAsync(keyTagsKey)
            .ConfigureAwait(false);

        if (tags.Length == 0)
        {
            return;
        }

        var batch = _database.CreateBatch();

        var tasks = new List<Task>(tags.Length + 1);

        foreach (var tag in tags)
        {
            tasks.Add(batch.SetRemoveAsync(
                _keyBuilder.BuildTag(tag.ToString()),
                key));
        }

        tasks.Add(batch.KeyDeleteAsync(keyTagsKey));

        batch.Execute();

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    public async Task InvalidateTagAsync(
        string tag,
        Func<string, CancellationToken, Task> removeEntry,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var tagKey = _keyBuilder.BuildTag(tag);

        var keys = await _database
            .SetMembersAsync(tagKey)
            .ConfigureAwait(false);

        if (keys.Length == 0)
        {
            return;
        }

        var tasks = new List<Task>(keys.Length);

        foreach (var key in keys)
        {
            tasks.Add(removeEntry(key.ToString(), cancellationToken));
        }

        await Task
            .WhenAll(tasks)
            .ConfigureAwait(false);

        await _database.KeyDeleteAsync(tagKey)
            .ConfigureAwait(false);
    }
}
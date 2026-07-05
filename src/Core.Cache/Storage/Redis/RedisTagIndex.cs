using Core.Cache.Storage.Abstractions;
using StackExchange.Redis;

namespace Core.Cache.Storage.Redis;

internal sealed class RedisTagIndex(
    IConnectionMultiplexer multiplexer,
    IKeyBuilder keyBuilder)
    : ICacheTagIndex<RedisStorage>
{
    private readonly IDatabase _database = multiplexer.GetDatabase();
    private readonly IKeyBuilder _keyBuilder = keyBuilder;

    public async Task AddAsync(
        string key,
        IReadOnlyCollection<string> tags,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var tasks = new List<Task>(tags.Count * 2);

        var fullKey = _keyBuilder.BuildCacheKey(key);

        foreach (var tag in tags)
        {
            tasks.Add(_database.SetAddAsync(
                _keyBuilder.BuildTag(tag),
                fullKey));

            tasks.Add(_database.SetAddAsync(
                _keyBuilder.BuildTagsIndex(key),
                tag));
        }

        await Task.WhenAll(tasks);
    }

    public async Task RemoveKeyAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var fullKey = _keyBuilder.BuildCacheKey(key);

        var tagsKey = _keyBuilder.BuildTagsIndex(key);

        var tags = await _database.SetMembersAsync(tagsKey);

        if (tags.Length == 0)
            return;

        var tasks = new List<Task>(tags.Length + 1);

        foreach (var tag in tags)
        {
            tasks.Add(
                _database.SetRemoveAsync(
                    _keyBuilder.BuildTag(tag.ToString()),
                    fullKey));
        }

        tasks.Add(_database.KeyDeleteAsync(tagsKey));

        await Task.WhenAll(tasks);
    }

    public async Task InvalidateTagAsync(
        string tag,
        Func<string, CancellationToken, Task> removeEntry,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var keys = await GetKeysAsync(tag, cancellationToken);

        if (keys.Count == 0)
            return;

        var tasks = keys
            .Select(key => removeEntry(key, cancellationToken));

        await Task.WhenAll(tasks);

        await _database.KeyDeleteAsync(
            _keyBuilder.BuildTag(tag));
    }

    public async Task<IReadOnlyCollection<string>> GetKeysAsync(
        string tag,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var members = await _database.SetMembersAsync(
            _keyBuilder.BuildTag(tag));

        return members
            .Select(x => x.ToString())
            .ToArray();
    }

    public Task<long> CountAsync(
        string tag,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return _database.SetLengthAsync(
            _keyBuilder.BuildTag(tag));
    }

    public async Task<bool> ExistsAsync(
        string tag,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await _database.KeyExistsAsync(
            _keyBuilder.BuildTag(tag));
    }
}
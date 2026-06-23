using Core.DistributedCache.Abstractions;
using Core.DistributedCache.Options;
using StackExchange.Redis;

namespace Core.DistributedCache.Storage.Redis;

internal class RedisCacheStorage(
    IConnectionMultiplexer redis,
    CacheOptions options,
    ICacheSerializer serializer) : ICoreCacheService
{
    private readonly IDatabase _database = redis.GetDatabase();
    private readonly ICacheSerializer _serializer = serializer;
    private readonly string _prefix = string.IsNullOrWhiteSpace(options.InstanceName)
        ? string.Empty
        : $"{options.InstanceName}:";

    private string GetFullKey(string key) => $"{_prefix}{key}";

    public IDatabase GetDatabase() => _database;

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        var value = await _database.StringGetAsync(GetFullKey(key));
        return value.HasValue ? _serializer.Deserialize<T>(value!) : default;
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
        => await _database.KeyDeleteAsync(GetFullKey(key));

    public async Task<bool> ExistsAsync(string key, CancellationToken ct = default)
        => await _database.KeyExistsAsync(GetFullKey(key));

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, string[]? tags = null, CancellationToken ct = default)
    {
        var fullKey = GetFullKey(key);
        var serializedValue = _serializer.Serialize(value);
        Expiration expiry = expiration.HasValue ? new Expiration(expiration.Value) : default;

        await _database.StringSetAsync(fullKey, serializedValue, expiry: expiry);

        if (tags != null && tags.Length > 0)
        {
            var batch = _database.CreateBatch();
            foreach (var tag in tags)
            {
                var tagKey = $"{_prefix}tag:{tag}";
                _ = batch.SetAddAsync(tagKey, key);
            }
            batch.Execute();
        }
    }

    public async Task InvalidateByTagAsync(string tag, CancellationToken ct = default)
    {
        var tagKey = $"{_prefix}tag:{tag}";

        var keys = await _database.SetMembersAsync(tagKey);

        if (keys.Length > 0)
        {
            var keysToDelete = keys.Select(k => (RedisKey)GetFullKey(k!)).ToArray();
            await _database.KeyDeleteAsync(keysToDelete);

            await _database.KeyDeleteAsync(tagKey);
        }
    }
}
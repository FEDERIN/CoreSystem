using Core.Cache.Abstractions;
using Core.Cache.Exceptions;
using Core.Cache.Storage.Abstractions;
using Core.Redis.Synchronization;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Core.Cache.Storage.Redis;

internal sealed class RedisStorage(
    IConnectionMultiplexer redis, 
    IPayloadSerializer payloadSerializer,
    IKeyBuilder keyBuilder,
    ICacheTagIndex<RedisStorage> tagIndex,
    IDistributedLockProvider distributedLockProvider,
    ILogger<RedisStorage> logger) : ICacheStorage
{
    private readonly IDatabase _database = redis.GetDatabase();
    private readonly ICacheTagIndex<RedisStorage> _tagIndex = tagIndex;
    private readonly IKeyBuilder _keyBuilder = keyBuilder;
    private readonly IPayloadSerializer _payloadSerializer = payloadSerializer;
    private readonly IDistributedLockProvider _distributedLockProvider = distributedLockProvider;

    private string GetFullKey(string key) => _keyBuilder.BuildCacheKey(key);

    public async Task<T?> GetAsync<T>(
        string key,
        CancellationToken ct = default)
    {
        var fullKey = GetFullKey(key);

        var payload = await _database.StringGetAsync(fullKey);

        if (payload.IsNullOrEmpty)
            return default;

        try
        {
            return _payloadSerializer.Deserialize<T>(payload);
        }
        catch (CacheDeserializationException ex)
        {
            logger.LogWarning(
                ex,
                "Corrupted cache entry detected for key '{Key}'. Removing it from Redis.",
                fullKey);

            await _database.KeyDeleteAsync(fullKey);

            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, CacheEntryOptions? options = null, TimeSpan? expiration = null, string[]? tags = null, CancellationToken ct = default)
    {
        var payload =
            _payloadSerializer.Serialize(value);

        Expiration expiry = expiration.HasValue ? new Expiration(expiration.Value) : default;

        await _database.StringSetAsync(GetFullKey(key), payload, expiry);

        if (tags is null || tags.Length == 0)
            return;

        await _tagIndex.AddAsync(key, tags, ct);
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
    {
        await _database.KeyDeleteAsync(GetFullKey(key));

        await _tagIndex.RemoveKeyAsync(key, ct);
    }

    public async Task InvalidateByTagAsync(
        string tag,
        CancellationToken ct = default)
    {
        await _tagIndex.InvalidateTagAsync(
            tag,
            async (key, _) =>
            {
                await _database.KeyDeleteAsync(key);
            },
            ct);
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken ct = default)
        => await _database.KeyExistsAsync(GetFullKey(key));

    public async Task<T?> GetOrAddAsync<T>(string key, Func<CancellationToken, Task<T>> factory, TimeSpan? expiration = null, string[]? tags = null, CancellationToken ct = default)
    {
        var cachedValue = await GetAsync<T>(key, ct);

        if (cachedValue is not null)
            return cachedValue;

        var lockKey = _keyBuilder.BuildLock(key);
        using (await _distributedLockProvider.AcquireAsync(lockKey, ct))
        {
            cachedValue = await GetAsync<T>(key, ct);

            if (cachedValue is not null)
                return cachedValue;

            var value = await factory(ct);

            if (value is not null)
                await SetAsync(key, value, null, expiration, tags, ct);

            return value;
        }
    }
}
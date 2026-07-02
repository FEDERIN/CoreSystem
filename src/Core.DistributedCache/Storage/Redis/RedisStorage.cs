using Core.DistributedCache.Abstractions;
using Core.DistributedCache.Storage.Abstractions;
using StackExchange.Redis;

namespace Core.DistributedCache.Storage.Redis;

internal sealed class RedisStorage(
    IConnectionMultiplexer redis, 
    IPayloadSerializer payloadSerializer,
    IKeyBuilder keyBuilder,
    ICacheTagIndex tagIndex,
    ICacheLockProvider lockProvider) : ICacheStorage
{
    private readonly IDatabase _database = redis.GetDatabase();
    private readonly ICacheTagIndex _tagIndex = tagIndex;
    private readonly IKeyBuilder _keyBuilder = keyBuilder;
    private readonly IPayloadSerializer _payloadSerializer = payloadSerializer;
    private readonly ICacheLockProvider _lockProvider = lockProvider;

    private string GetFullKey(string key) => _keyBuilder.BuildCacheKey(key);

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        var payload = await _database.StringGetAsync(GetFullKey(key));

        if (payload.IsNullOrEmpty)
            return default;

        return _payloadSerializer.Deserialize<T>(payload);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, string[]? tags = null, CancellationToken ct = default)
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
        => await _database.KeyDeleteAsync(GetFullKey(key));

    public async Task InvalidateByTagAsync(string tag, CancellationToken ct = default)
    {
        await _tagIndex.InvalidateTagAsync(
            tag,
            (key, _) =>
            {
                _database.KeyDeleteAsync(key);
                return Task.CompletedTask;
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

        using (await _lockProvider.AcquireAsync(key, ct))
        {
            cachedValue = await GetAsync<T>(key, ct);

            if (cachedValue is not null)
                return cachedValue;

            var value = await factory(ct);

            if (value is not null)
                await SetAsync(key, value, expiration, tags, ct);

            return value;
        }
    }
}
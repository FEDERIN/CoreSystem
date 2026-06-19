using Core.DistributedCache.Abstractions;
using Core.DistributedCache.Options;
using StackExchange.Redis;

namespace Core.DistributedCache.Storage.Redis;

public class RedisCacheStorage(
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

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken ct = default)
    {
        var serializedValue = _serializer.Serialize(value);
        Expiration expiry = expiration.HasValue ? new Expiration(expiration.Value) : default;

        await _database.StringSetAsync(
                GetFullKey(key),
                serializedValue,
                expiry: expiry
            );

    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
        => await _database.KeyDeleteAsync(GetFullKey(key));

    public async Task<bool> ExistsAsync(string key, CancellationToken ct = default)
        => await _database.KeyExistsAsync(GetFullKey(key));
}
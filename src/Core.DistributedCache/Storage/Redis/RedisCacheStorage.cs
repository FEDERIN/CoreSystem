using Core.DistributedCache.Abstractions;
using Core.DistributedCache.Options;
using StackExchange.Redis;

namespace Core.DistributedCache.Storage.Redis;

internal class RedisCacheStorage(
    IConnectionMultiplexer redis,
    CacheOptions options,
    ICacheSerializerFactory serializerFactory) : ICoreCacheService
{
    private readonly IDatabase _database = redis.GetDatabase();
    private readonly ICacheSerializerFactory _serializerFactory = serializerFactory;
    private readonly string _prefix = string.IsNullOrWhiteSpace(options.InstanceName)
        ? string.Empty
        : $"{options.InstanceName}:";

    private string GetFullKey(string key) => $"{_prefix}{key}";
    public IDatabase GetDatabase() => _database;


    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        byte[]? buffer = await _database.StringGetAsync($"{_prefix}{key}");
        if (buffer == null || buffer.Length == 0) return default;

        // 1. Detección inteligente: ¿El primer byte es un tipo válido?
        if (Enum.IsDefined(typeof(SerializerType), buffer[0]))
        {
            SerializerType type = (SerializerType)buffer[0];
            return _serializerFactory.GetSerializer(type).Deserialize<T>(buffer.AsSpan(1).ToArray());
        }

        // 2. Fallback para datos legados (JSON puro)
        return _serializerFactory.GetSerializer(SerializerType.Json).Deserialize<T>(buffer);
    }
    
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, string[]? tags = null, CancellationToken ct = default)
    {
        var serializer = _serializerFactory.GetSerializer(options.SerializerType);

        byte[] payload = serializer.Serialize(value);

        byte[] dataToStore;

        if (serializer.RequiresHeader)
        {
            dataToStore = new byte[payload.Length + 1];
            dataToStore[0] = (byte)options.SerializerType;
            payload.AsSpan().CopyTo(dataToStore.AsSpan(1));
        }
        else
        {
            dataToStore = payload;
        }

        Expiration expiry = expiration.HasValue ? new Expiration(expiration.Value) : default;

        await _database.StringSetAsync(GetFullKey(key), dataToStore, expiry);

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

    public async Task RemoveAsync(string key, CancellationToken ct = default)
        => await _database.KeyDeleteAsync(GetFullKey(key));

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

    public async Task<bool> ExistsAsync(string key, CancellationToken ct = default)
        => await _database.KeyExistsAsync(GetFullKey(key));

    public async Task<T?> GetOrAddAsync<T>(string key, Func<CancellationToken, Task<T>> factory, TimeSpan? expiration = null, string[]? tags = null, CancellationToken ct = default)
    {
        var cachedValue = await GetAsync<T>(key, ct);

        if (cachedValue is not null)
            return cachedValue;

        var lockKey = $"{GetFullKey(key)}:lock";
        var token = Guid.NewGuid().ToString();

        if (await _database.LockTakeAsync(lockKey, token, TimeSpan.FromSeconds(10)))
        {
            try
            {
                cachedValue = await GetAsync<T>(key, ct);

                if (cachedValue is not null) 
                    return cachedValue;

                
                var value = await factory(ct);

                if (value is not null) 
                    await SetAsync(key, value, expiration, tags, ct);

                return value;
            }
            finally
            {
                await _database.LockReleaseAsync(lockKey, token);
            }
        }
        else
        {
            await Task.Delay(100, ct);
            return await GetAsync<T>(key, ct);
        }
    }
}
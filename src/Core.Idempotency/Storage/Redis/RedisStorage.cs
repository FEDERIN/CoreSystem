using Core.Idempotency.Abstractions;
using Core.Idempotency.Models;
using Core.Idempotency.Storage.Abstractions;
using Core.Serialization.Abstractions;
using StackExchange.Redis;

namespace Core.Idempotency.Storage.Redis;

internal sealed class RedisStorage(
    IConnectionMultiplexer redis,
    IKeyBuilder keyBuilder,
    IPayloadSerializer payloadSerializer)
    : IIdempotencyStorage
{
    private readonly IDatabase _database = redis.GetDatabase();

    public async Task<IdempotencyResponse?> GetAsync(
        string key,
        CancellationToken ct = default)
    {
        var redisKey =
            keyBuilder.BuildCacheKey(key);

        var payload =
            await _database.StringGetAsync(redisKey);

        if (payload.IsNullOrEmpty)
        {
            return null;
        }

        return payloadSerializer.Deserialize<IdempotencyResponse>(payload);
    }

    public async Task SetAsync(
        string key,
        IdempotencyResponse response,
        TimeSpan? expiration = null,
        CancellationToken ct = default)
    {
        var redisKey =
            keyBuilder.BuildCacheKey(key);

        var payload =
            payloadSerializer.Serialize(response);

        Expiration expiry = expiration.HasValue ? new Expiration(expiration.Value) : default;

        await _database.StringSetAsync(
            redisKey,
            payload,
            expiry);
    }
}
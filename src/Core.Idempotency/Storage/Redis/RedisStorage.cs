using Core.Idempotency.Abstractions;
using Core.Idempotency.Diagnostics;
using Core.Idempotency.Models;
using Core.Idempotency.Storage.Abstractions;
using Core.Redis.Synchronization;
using Core.Serialization.Abstractions;
using StackExchange.Redis;
using System.Diagnostics;

namespace Core.Idempotency.Storage.Redis;

internal sealed class RedisStorage(
    IConnectionMultiplexer redis,
    IKeyBuilder keyBuilder,
    IPayloadSerializer payloadSerializer,
    IDistributedLockProvider distributedLockProvider,
    IdempotencyMetrics metrics)
    : IIdempotencyStorage
{
    private readonly IDatabase _database = redis.GetDatabase();

    public async Task<IdempotencyResponse?> GetAsync(
        string key,
        CancellationToken ct = default)
    {
        var redisKey = keyBuilder.BuildCacheKey(key);

        long start = Stopwatch.GetTimestamp();

        try
        {
            var payload = await _database.StringGetAsync(redisKey);

            if (payload.IsNullOrEmpty)
                return null;

            return payloadSerializer.Deserialize<IdempotencyResponse>(payload);
        }
        finally
        {
            metrics.RecordStorageReadDuration(
                Stopwatch.GetElapsedTime(start).TotalMilliseconds);
        }
    }

    public async Task SetAsync(
        string key,
        IdempotencyResponse response,
        TimeSpan? expiration = null,
        CancellationToken ct = default)
    {
        var redisKey = keyBuilder.BuildCacheKey(key);
        var lockKey = keyBuilder.BuildLock(key);
        var payload = payloadSerializer.Serialize(response);

        Expiration expiry = expiration.HasValue
            ? new Expiration(expiration.Value)
            : default;

        long start = Stopwatch.GetTimestamp();

        try
        {
            using (await distributedLockProvider.AcquireAsync(lockKey, ct))
            {
                if (await _database.KeyExistsAsync(redisKey))
                    return;

                await _database.StringSetAsync(
                    redisKey,
                    payload,
                    expiry);

                metrics.RecordStorageWrite();
                metrics.RecordPayloadSize(payload.Length);
            }
        }
        finally
        {
            metrics.RecordStorageWriteDuration(
                Stopwatch.GetElapsedTime(start).TotalMilliseconds);
        }
    }
}
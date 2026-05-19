using StackExchange.Redis;
using System.Text.Json;
using Core.Idempotency.Models;

namespace Core.Idempotency.Storage.Redis;

public class RedisIdempotencyStorage(IConnectionMultiplexer redis) : IIdempotencyStorage
{
    private readonly IDatabase _db = redis.GetDatabase();
    private const string KeyPrefix = "idemp:";

    public async Task<IdempotencyResponse?> GetAsync(string key)
    {
        var value = await _db.StringGetAsync($"{KeyPrefix}{key}");
        if (!value.HasValue) return null;

        return JsonSerializer.Deserialize<IdempotencyResponse>(value!);
    }

    public async Task SaveAsync(string key, IdempotencyResponse response, TimeSpan expiration)
    {
        var serialized = JsonSerializer.Serialize(response);
        await _db.StringSetAsync($"{KeyPrefix}{key}", serialized, expiration);
    }
}
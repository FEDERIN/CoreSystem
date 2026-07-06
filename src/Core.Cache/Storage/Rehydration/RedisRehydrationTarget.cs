using Core.Cache.Storage.Redis;

namespace Core.Cache.Storage.Rehydration;

internal sealed class RedisRehydrationTarget(
    RedisStorage redisStorage)
    : IRehydrationTarget
{
    public Task StoreAsync(
        CacheRehydrationEntry entry,
        CancellationToken ct = default)
    {
        return redisStorage.SetAsync(
            entry.Key,
            entry.Value,
            expiration: entry.RemainingExpiration,
            ct: ct);
    }
}
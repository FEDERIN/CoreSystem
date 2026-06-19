namespace Core.DistributedCache.Abstractions;

public static class CoreCacheExtensions
{
    public static async Task<T?> GetOrAddAsync<T>(
        this ICoreCacheService cache,
        string key,
        Func<Task<T>> factory,
        TimeSpan? expiration = null,
        CancellationToken ct = default)
    {
        var cachedValue = await cache.GetAsync<T>(key, ct);
        if (cachedValue is not null)
        {
            return cachedValue;
        }

        var value = await factory();

        if (value is not null)
        {
            await cache.SetAsync(key, value, expiration, ct);
        }

        return value;
    }
}
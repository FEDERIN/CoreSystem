using Core.DistributedCache.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace Core.DistributedCache.Storage.Memory;

public class MemoryCacheStorage(IMemoryCache memoryCache) : ICoreCacheService
{
    private readonly IMemoryCache _memoryCache = memoryCache;

    public Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        _memoryCache.TryGetValue(key, out T? value);
        return Task.FromResult(value);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken ct = default)
    {
        var options = new MemoryCacheEntryOptions();
        if (expiration.HasValue)
        {
            options.SetAbsoluteExpiration(expiration.Value);
        }

        _memoryCache.Set(key, value, options);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken ct = default)
    {
        _memoryCache.Remove(key);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string key, CancellationToken ct = default)
    {
        return Task.FromResult(_memoryCache.TryGetValue(key, out _));
    }
}

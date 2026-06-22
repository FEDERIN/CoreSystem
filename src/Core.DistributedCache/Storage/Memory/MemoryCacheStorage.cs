using Core.DistributedCache.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;

namespace Core.DistributedCache.Storage.Memory;

public class MemoryCacheStorage(IMemoryCache memoryCache) : ICoreCacheService
{
    private readonly IMemoryCache _memoryCache = memoryCache;
    private readonly ConcurrentDictionary<string, ConcurrentHashSet<string>> _tagIndex = new();

    public Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        _memoryCache.TryGetValue(key, out T? value);
        return Task.FromResult(value);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, string[]? tags = null, CancellationToken ct = default)
    {
        var options = new MemoryCacheEntryOptions();
        if (expiration.HasValue) options.SetAbsoluteExpiration(expiration.Value);

        _memoryCache.Set(key, value, options);

        if (tags != null)
        {
            foreach (var tag in tags)
            {
                var keys = _tagIndex.GetOrAdd(tag, _ => []);
                keys.Add(key);
            }
        }
        await Task.CompletedTask;
    }

    public async Task InvalidateByTagAsync(string tag, CancellationToken ct = default)
    {
        if (_tagIndex.TryRemove(tag, out var keys))
        {
            foreach (var key in keys)
            {
                _memoryCache.Remove(key);
            }
        }
        await Task.CompletedTask;
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

internal class ConcurrentHashSet<T> : ConcurrentDictionary<T, byte> where T : notnull
{
    public void Add(T item) => TryAdd(item, 0);
    public new IEnumerator<T> GetEnumerator() => Keys.GetEnumerator();
}
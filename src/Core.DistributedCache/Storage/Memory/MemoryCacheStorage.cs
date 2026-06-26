using Core.DistributedCache.Abstractions;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;

namespace Core.DistributedCache.Storage.Memory;

internal class MemoryCacheStorage(IMemoryCache memoryCache) : ICoreCacheService
{
    private readonly IMemoryCache _memoryCache = memoryCache;
    private readonly ConcurrentDictionary<string, ConcurrentHashSet<string>> _tagIndex = new();
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

    public Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        _memoryCache.TryGetValue(key, out T? value);
        return Task.FromResult(value);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, string[]? tags = null, CancellationToken ct = default)
    {
        var options = new MemoryCacheEntryOptions();

        if (expiration.HasValue)
            options.SetAbsoluteExpiration(expiration.Value);

        if (tags != null && tags.Length > 0)
        {
            options.RegisterPostEvictionCallback((evictedKey, _, reason, _) =>
            {
                if (reason != EvictionReason.Removed)
                {
                    foreach (var tag in tags)
                    {
                        if (_tagIndex.TryGetValue(tag, out var keys))
                        {
                            keys.TryRemove((string)evictedKey, out _);

                            if (keys.IsEmpty)
                            {
                                _tagIndex.TryRemove(tag, out _);
                            }
                        }
                    }
                }
            });
        }

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

    public Task RemoveAsync(string key, CancellationToken ct = default)
    {
        _memoryCache.Remove(key);
        return Task.CompletedTask;
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

    public Task<bool> ExistsAsync(string key, CancellationToken ct = default)
    {
        return Task.FromResult(_memoryCache.TryGetValue(key, out _));
    }

    public async Task<T?> GetOrAddAsync<T>(
            string key,
            Func<CancellationToken, Task<T>> factory,
            TimeSpan? expiration = null,
            string[]? tags = null,
            CancellationToken ct = default)
    {
        if (_memoryCache.TryGetValue(key, out T? value))
            return value;

        var myLock = _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
        await myLock.WaitAsync(ct);

        try
        {
            if (_memoryCache.TryGetValue(key, out value))
                return value;

            value = await factory(ct);

            if (value != null)
            {
                await SetAsync(key, value, expiration, tags, ct);
            }

            return value;
        }
        finally
        {
            myLock.Release();
        }
    }
}

internal class ConcurrentHashSet<T> : ConcurrentDictionary<T, byte> where T : notnull
{
    public void Add(T item) => TryAdd(item, 0);
    public new IEnumerator<T> GetEnumerator() => Keys.GetEnumerator();
}
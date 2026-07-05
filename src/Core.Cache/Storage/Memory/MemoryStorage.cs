using Core.Cache.Abstractions;
using Core.Cache.Storage.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace Core.Cache.Storage.Memory;

internal sealed class MemoryStorage(IMemoryCache memoryCache, ICacheTagIndex<MemoryStorage> tagIndex,
    ICacheKeyTracker tracker, ICacheLockProvider<MemoryStorage> lockProvider, ICacheEntryFactory entryFactory) : ICacheStorage
{
    private readonly IMemoryCache _memoryCache = memoryCache;
    private readonly ICacheTagIndex<MemoryStorage> _tagIndex = tagIndex;
    private readonly ICacheKeyTracker _tracker = tracker;
    private readonly ICacheLockProvider<MemoryStorage> _lockProvider = lockProvider;
    private readonly ICacheEntryFactory _entryFactory = entryFactory;

    public Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        return Task.FromResult(
            TryGetValue(key, out T? value)
                ? value
                : default);
    }

    public async Task SetAsync<T>(string key, T value, CacheEntryOptions? options = null, TimeSpan? expiration = null, string[]? tags = null, CancellationToken ct = default)
    {
        var cacheOptions = new MemoryCacheEntryOptions();

        if (expiration.HasValue)
            cacheOptions.SetAbsoluteExpiration(expiration.Value);

        cacheOptions.RegisterPostEvictionCallback((evictedKey, _, _, _) =>
        {
            var key = (string)evictedKey;

            _ = _tagIndex.RemoveKeyAsync(key);
            _tracker.Untrack(key);
        });

        var wrapper = _entryFactory.Create(value, options ?? CacheEntryOptions.Default);

        _memoryCache.Set(key, wrapper, cacheOptions);

        if (wrapper.Origin == CacheProviderType.Redis)
        {
            _tracker.Track(key);
        }

        if (tags is null || tags.Length == 0)
            return;

        await _tagIndex.AddAsync(key, tags, ct);
    }

    public Task RemoveAsync(string key, CancellationToken ct = default)
    {
        _memoryCache.Remove(key);
        return Task.CompletedTask;
    }

    public async Task InvalidateByTagAsync(string tag, CancellationToken ct = default)
    {
        await _tagIndex.InvalidateTagAsync(
            tag,
            (key, _) =>
            {
                _memoryCache.Remove(key);
                return Task.CompletedTask;
            },
            ct);
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
        if (TryGetValue(key, out T? value))
        {
            return value;
        }

        using (await _lockProvider.AcquireAsync(key, ct))
        {
            if (TryGetValue(key, out value))
            {
                return value;
            }

            value = await factory(ct);

            if (value is null)
            {
                return default;
            }

            await SetAsync(key, value, null, expiration, tags, ct);

            return value;
        }
    }

    private bool TryGetValue<T>(string key, out T? value)
    {
        if (_memoryCache.TryGetValue(key, out object? entry) &&
            _entryFactory.TryUnwrap(entry, out value))
        {
            return true;
        }

        value = default;
        return false;
    }
}
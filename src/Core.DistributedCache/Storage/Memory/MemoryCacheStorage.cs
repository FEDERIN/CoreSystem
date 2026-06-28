using Core.DistributedCache.Abstractions;
using Core.DistributedCache.Storage.Memory.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace Core.DistributedCache.Storage.Memory;

internal class MemoryCacheStorage(IMemoryCache memoryCache, IMemoryTagIndex tagIndex,
    IMemoryKeyTracker tracker, IKeyLockProvider keyLock) : ICoreCacheService
{
    private readonly IMemoryCache _memoryCache = memoryCache;
    private readonly IMemoryTagIndex _tagIndex = tagIndex;
    private readonly IMemoryKeyTracker _tracker = tracker;
    private readonly IKeyLockProvider _keyLock = keyLock;

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        if (_memoryCache.TryGetValue(key, out CacheEntryWrapper<T>? wrapper) && wrapper != null)
        {
            return wrapper.Value;
        }

        return default;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, string[]? tags = null, CancellationToken ct = default)
    {
        var options = new MemoryCacheEntryOptions();

        if (expiration.HasValue)
            options.SetAbsoluteExpiration(expiration.Value);

        options.RegisterPostEvictionCallback((evictedKey, _, reason, _) =>
        {
            if (reason != EvictionReason.Removed)
                _tagIndex.RemoveKey((string)evictedKey);
        });


        CacheEntryWrapper<T> wrapper;
        var isRedis = false;

        if (value is CacheEntryWrapper<T> wrapped)
        {
            wrapper = wrapped;

            if (wrapped.Origin == CacheProviderType.Redis)
                isRedis = true;
        }
        else
        {
            wrapper = new CacheEntryWrapper<T>
            {
                Value = value,
                Origin = CacheProviderType.Memory,
            };
        }

        _memoryCache.Set(key, wrapper, options);

        if(isRedis)
            _tracker.Track(key);
        
        if (tags != null) 
            _tagIndex.AddTags(key, tags);


        await Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken ct = default)
    {
        _memoryCache.Remove(key);
        return Task.CompletedTask;
    }

    public async Task InvalidateByTagAsync(string tag, CancellationToken ct = default)
    {
        _tagIndex.RemoveByTag(tag, key => _memoryCache.Remove(key));
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

        using (await _keyLock.AcquireAsync(key, ct))
        {
            if (_memoryCache.TryGetValue(key, out value))
                return value;

            value = await factory(ct);
            if (value != null)
                await SetAsync(key, value, expiration, tags, ct);

            return value;
        } 
    }

    public IEnumerable<string> GetTrackedKeys()
    {
        return _tracker.GetAllTrackedKeys();
    }

    internal CacheEntryWrapper<T>? GetWrapper<T>(string key)
    {
        return _memoryCache.TryGetValue(key, out CacheEntryWrapper<T>? wrapper) ? wrapper : null;
    }
}
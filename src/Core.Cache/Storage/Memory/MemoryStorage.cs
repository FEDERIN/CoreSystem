using Core.Cache.Abstractions;
using Core.Cache.Storage.Abstractions;
using Core.Cache.Storage.Rehydration.Tracking;
using Core.Memory.Synchronization;
using Microsoft.Extensions.Caching.Memory;

namespace Core.Cache.Storage.Memory;

internal sealed class MemoryStorage(
    IMemoryCache memoryCache,
    ICacheTagIndex<MemoryStorage> tagIndex,
    IMemoryLockProvider lockProvider,
    ICacheEntryFactory entryFactory,
    ICacheEntryInspector entryInspector,
    IRehydrationTracker rehydrationTracker)
    : ICacheStorage
{
    private readonly IMemoryCache _memoryCache = memoryCache;
    private readonly ICacheTagIndex<MemoryStorage> _tagIndex = tagIndex;
    private readonly IMemoryLockProvider _lockProvider = lockProvider;
    private readonly ICacheEntryFactory _entryFactory = entryFactory;
    private readonly ICacheEntryInspector _entryInspector = entryInspector;
    private readonly IRehydrationTracker _rehydrationTracker = rehydrationTracker;

    public Task<T?> GetAsync<T>(
        string key,
        CancellationToken ct = default)
    {
        return Task.FromResult(
            TryGetValue(key, out T? value)
                ? value
                : default);
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        CacheEntryOptions? options = null,
        TimeSpan? expiration = null,
        string[]? tags = null,
        CancellationToken ct = default)
    {
        DateTimeOffset? absoluteExpiration = null;

        if (expiration.HasValue)
        {
            absoluteExpiration =
                DateTimeOffset.UtcNow.Add(expiration.Value);
        }

        var wrapper = _entryFactory.Create(
            value,
            options ?? CacheEntryOptions.Default,
            absoluteExpiration);

        var cacheOptions = new MemoryCacheEntryOptions();

        if (expiration.HasValue)
        {
            cacheOptions.SetAbsoluteExpiration(expiration.Value);
        }

        cacheOptions.RegisterPostEvictionCallback((evictedKey, _, _, _) =>
        {
            var cacheKey = (string)evictedKey;

            _ = _tagIndex.RemoveKeyAsync(cacheKey);

            _rehydrationTracker.Untrack(cacheKey);
        });

        _memoryCache.Set(key, wrapper, cacheOptions);

        _rehydrationTracker.Track(
            key,
            wrapper.Origin);

        if (tags is { Length: > 0 })
        {
            await _tagIndex.AddAsync(
                key,
                tags,
                ct);
        }
    }

    public async Task RemoveAsync(
        string key,
        CancellationToken ct = default)
    {
        await _tagIndex.RemoveKeyAsync(
            key,
            ct);

        _rehydrationTracker.Untrack(key);

        _memoryCache.Remove(key);
    }

    public Task<bool> ExistsAsync(
        string key,
        CancellationToken ct = default)
    {
        return Task.FromResult(
            _memoryCache.TryGetValue(key, out _));
    }

    public Task InvalidateByTagAsync(
        string tag,
        CancellationToken ct = default)
    {
        return _tagIndex.InvalidateTagAsync(
            tag,
            (key, _) =>
            {
                _rehydrationTracker.Untrack(key);

                _memoryCache.Remove(key);

                return Task.CompletedTask;
            },
            ct);
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

            await SetAsync(
                key,
                value,
                null,
                expiration,
                tags,
                ct);

            return value;
        }
    }

    private bool TryGetValue<T>(
        string key,
        out T? value)
    {
        if (_memoryCache.TryGetValue(key, out object? entry) &&
            _entryInspector.TryGetValue(entry, out value))
        {
            return true;
        }

        value = default;
        return false;
    }
}
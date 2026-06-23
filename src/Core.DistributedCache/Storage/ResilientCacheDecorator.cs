using Core.DistributedCache.Abstractions;
using Core.DistributedCache.Storage.Memory;
using Core.DistributedCache.Storage.Redis;

namespace Core.DistributedCache.Storage;

internal class ResilientCacheDecorator : ICoreCacheService, IDisposable
{
    public bool IsRedisHealthy { get; private set; } = true;
    private readonly RedisCacheStorage _redis;
    private readonly MemoryCacheStorage _memory;
    private readonly Timer _healthCheckTimer;

    public ResilientCacheDecorator(RedisCacheStorage redis, MemoryCacheStorage memory)
    {
        _redis = redis;
        _memory = memory;
        _healthCheckTimer = new Timer(CheckRedisHealth, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30));
    }

    private void CheckRedisHealth(object? state)
    {
        try
        {
            _redis.GetDatabase().Ping();
            if (!IsRedisHealthy)
            {
                IsRedisHealthy = true;
            }
        }
        catch { 
            IsRedisHealthy = false; 
        }
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken ct = default)
    {
        if (IsRedisHealthy)
        {
            try { return await _redis.ExistsAsync(key, ct); }
            catch { IsRedisHealthy = false; }
        }
        return await _memory.ExistsAsync(key, ct);
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        if (IsRedisHealthy)
        {
            try { return await _redis.GetAsync<T>(key, ct); }
            catch { IsRedisHealthy = false; }
        }
        return await _memory.GetAsync<T>(key, ct);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? exp = null, string[]? tags = null, CancellationToken ct = default)
    {
        if (IsRedisHealthy)
        {
            try { await _redis.SetAsync(key, value, exp, tags, ct); return; }
            catch { IsRedisHealthy = false; }
        }
        await _memory.SetAsync(key, value, exp, tags, ct);
    }

    public async Task InvalidateByTagAsync(string tag, CancellationToken ct = default)
    {
        if (IsRedisHealthy)
        {
            try { await _redis.InvalidateByTagAsync(tag, ct); return; }
            catch { IsRedisHealthy = false; }
        }
        await _memory.InvalidateByTagAsync(tag, ct);
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
    {
        if (IsRedisHealthy)
        {
            try { await _redis.RemoveAsync(key, ct); }
            catch { IsRedisHealthy = false; }
        }
        await _memory.RemoveAsync(key, ct);
    }

    public void Dispose()
    {
        _healthCheckTimer?.Dispose();

        GC.SuppressFinalize(this);
    }
}
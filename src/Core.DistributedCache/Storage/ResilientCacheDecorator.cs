using Core.DistributedCache.Abstractions;
using Core.DistributedCache.Storage.Memory;
using Core.DistributedCache.Storage.Redis;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using StackExchange.Redis;

namespace Core.DistributedCache.Storage;

internal class ResilientCacheDecorator(
    RedisCacheStorage redisStorage,
    MemoryCacheStorage memoryStorage,
    ILogger<ResilientCacheDecorator> logger) : ICoreCacheService
{
    private readonly RedisCacheStorage _redisStorage = redisStorage;
    private readonly MemoryCacheStorage _memoryStorage = memoryStorage;
    private readonly ILogger<ResilientCacheDecorator> _logger = logger;
    private readonly AsyncCircuitBreakerPolicy _circuitBreaker = Policy
            .Handle<RedisConnectionException>()
            .Or<TimeoutException>()
            .Or<RedisCommandException>()
            .AdvancedCircuitBreakerAsync(
                failureThreshold: 0.5,
                samplingDuration: TimeSpan.FromSeconds(30),
                minimumThroughput: 10,
                durationOfBreak: TimeSpan.FromSeconds(15)
            );

    public bool IsRedisHealthy => _circuitBreaker.CircuitState != CircuitState.Open;

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        return await ExecuteAsync(
            async () => await _redisStorage.GetAsync<T>(key, ct),
            async () => await _memoryStorage.GetAsync<T>(key)
        );
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? exp = null, string[]? tags = null, CancellationToken ct = default)
    {
        try
        {
            await _circuitBreaker.ExecuteAsync(() => _redisStorage.SetAsync(key, value, exp, tags, ct));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Redis Set falló. Degradando a MemoryCache con origen marcado.");

            var wrappedValue = new CacheEntryWrapper<T>
            {
                Value = value,
                Origin = CacheProviderType.Redis
            };

            await _memoryStorage.SetAsync(key, wrappedValue, exp, tags, ct);
        }
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken ct = default)
        => await ExecuteAsync(() => _redisStorage.ExistsAsync(key, ct), () => _memoryStorage.ExistsAsync(key, ct));

    public async Task RemoveAsync(string key, CancellationToken ct = default)
        => await ExecuteActionAsync(() => 
        _redisStorage.RemoveAsync(key, ct), () => _memoryStorage.RemoveAsync(key, ct));

    public async Task InvalidateByTagAsync(string tag, CancellationToken ct = default)
        => await ExecuteActionAsync(() => 
        _redisStorage.InvalidateByTagAsync(tag, ct), () => _memoryStorage.InvalidateByTagAsync(tag, ct));

    public async Task<T?> GetOrAddAsync<T>(string key, Func<CancellationToken, Task<T>> factory, TimeSpan? expiration = null, string[]? tags = null, CancellationToken ct = default)
        => await ExecuteAsync(() => 
        _redisStorage.GetOrAddAsync(key, factory, expiration, tags, ct), () => _memoryStorage.GetOrAddAsync(key, factory, expiration, tags, ct));

    private async Task ExecuteActionAsync(Func<Task> redisAction, Func<Task> fallbackAction)
    {
        try
        {
            await _circuitBreaker.ExecuteAsync(redisAction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Redis operation failed. Falling back to MemoryCache.");
            await fallbackAction();
        }
    }

    private async Task<T?> ExecuteAsync<T>(
    Func<Task<T?>> redisAction,
    Func<Task<T?>> fallbackAction)
    {
        try
        {
            return await _circuitBreaker.ExecuteAsync(redisAction);
        }
        catch (Exception ex) when (ex is BrokenCircuitException or RedisException or TimeoutException)
        {
            _logger.LogWarning(ex, "Redis unavailable. Using MemoryCache fallback.");
            return await fallbackAction();
        }
    }
}
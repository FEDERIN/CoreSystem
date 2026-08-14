using Core.Cache.Storage;
using Core.Cache.Storage.Abstractions;

namespace Core.Cache.Rehydration.UnitTests.DependencyInjection;

internal sealed class FakeExternalCacheStorage
    : IExternalCacheStorage
{
    public Task<T?> GetAsync<T>(
        string key,
        CancellationToken ct = default)
        => Task.FromResult<T?>(default);

    public Task SetAsync<T>(
        string key,
        T value,
        CacheEntryOptions? options = null,
        TimeSpan? expiration = null,
        string[]? tags = null,
        CancellationToken ct = default)
        => Task.CompletedTask;

    public Task RemoveAsync(
        string key,
        CancellationToken ct = default)
        => Task.CompletedTask;

    public Task<bool> ExistsAsync(
        string key,
        CancellationToken ct = default)
        => Task.FromResult(false);

    public Task InvalidateByTagAsync(
        string tag,
        CancellationToken ct = default)
        => Task.CompletedTask;

    public async Task<T?> GetOrAddAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        CacheEntryOptions? options = null,
        TimeSpan? expiration = null,
        string[]? tags = null,
        CancellationToken ct = default)
    {
        return await factory(ct);
    }
}
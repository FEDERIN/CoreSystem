using Core.Cache.Abstractions;

namespace Core.Cache.Services;

internal sealed class NoOpCoreCache : ICoreCache
{
    public Task<bool> ExistsAsync(
        string key,
        CancellationToken ct = default)
    {
        return Task.FromResult(false);
    }

    public Task<T?> GetAsync<T>(
        string key,
        CancellationToken ct = default)
    {
        return Task.FromResult<T?>(default);
    }

    public async Task<T?> GetOrAddAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? expiration = null,
        string[]? tags = null,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(factory);

        return await factory(ct);
    }

    public Task InvalidateByTagAsync(
        string tag,
        CancellationToken ct = default)
    {
        return Task.CompletedTask;
    }

    public Task RemoveAsync(
        string key,
        CancellationToken ct = default)
    {
        return Task.CompletedTask;
    }

    public Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        string[]? tags = null,
        CancellationToken ct = default)
    {
        return Task.CompletedTask;
    }
}
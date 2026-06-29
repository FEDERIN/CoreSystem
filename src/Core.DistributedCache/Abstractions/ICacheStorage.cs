namespace Core.DistributedCache.Abstractions;

public interface ICacheStorage
{
    Task<T?> GetAsync<T>(
        string key,
        CancellationToken ct = default);

    Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        string[]? tags = null,
        CancellationToken ct = default);

    Task RemoveAsync(
        string key,
        CancellationToken ct = default);

    Task<bool> ExistsAsync(
        string key,
        CancellationToken ct = default);

    Task InvalidateByTagAsync(
        string tag,
        CancellationToken ct = default);

    Task<T?> GetOrAddAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? expiration = null,
        string[]? tags = null,
        CancellationToken ct = default);
}
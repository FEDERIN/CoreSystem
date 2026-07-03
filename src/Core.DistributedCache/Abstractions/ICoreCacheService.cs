namespace Core.DistributedCache.Abstractions;

/// <summary>
/// Provides a unified abstraction for distributed caching operations.
/// </summary>
public interface ICoreCacheService
{
    /// <summary>
    /// Retrieves an item from the cache.
    /// </summary>
    /// <typeparam name="T">The type of the object to retrieve.</typeparam>
    /// <param name="key">The unique identifier for the cache entry.</param>
    /// <param name="ct">The <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>The cached value, or <c>null</c> if the key does not exist.</returns>
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default);

    /// <summary>
    /// Adds or updates an item in the cache.
    /// </summary>
    /// <typeparam name="T">The type of the object to cache.</typeparam>
    /// <param name="key">The unique identifier for the cache entry.</param>
    /// <param name="value">The object to be cached.</param>
    /// <param name="expiration">Optional sliding or absolute expiration time. Defaults to cache provider defaults.</param>
    /// <param name="tags">Optional tags to group related cache entries for batch invalidation.</param>
    /// <param name="ct">The <see cref="CancellationToken"/> to observe.</param>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, string[]? tags = null, CancellationToken ct = default);

    /// <summary>
    /// Removes a specific item from the cache.
    /// </summary>
    /// <param name="key">The unique identifier for the cache entry.</param>
    /// <param name="ct">The <see cref="CancellationToken"/> to observe.</param>
    Task RemoveAsync(string key, CancellationToken ct = default);

    /// <summary>
    /// Invalidates all entries associated with the specified tag.
    /// </summary>
    /// <param name="tag">The tag to invalidate.</param>
    /// <param name="ct">The <see cref="CancellationToken"/> to observe.</param>
    Task InvalidateByTagAsync(string tag, CancellationToken ct = default);

    /// <summary>
    /// Checks if a key exists in the cache.
    /// </summary>
    /// <param name="key">The unique identifier for the cache entry.</param>
    /// <param name="ct">The <see cref="CancellationToken"/> to observe.</param>
    /// <returns><c>true</c> if the key exists; otherwise, <c>false</c>.</returns>
    Task<bool> ExistsAsync(string key, CancellationToken ct = default);

    /// <summary>
    /// Retrieves an item from the cache, or adds it if it is missing or expired.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="key">The unique identifier for the cache entry.</param>
    /// <param name="factory">The asynchronous function to execute if the item is not found.</param>
    /// <param name="expiration">Optional expiration time.</param>
    /// <param name="tags">Optional tags for the item.</param>
    /// <param name="ct">The <see cref="CancellationToken"/> to observe.</param>
    /// <returns>The existing or newly generated value.</returns>
    Task<T?> GetOrAddAsync<T>(
        string key,
        Func<CancellationToken, Task<T>> factory,
        TimeSpan? expiration = null,
        string[]? tags = null,
        CancellationToken ct = default);
}
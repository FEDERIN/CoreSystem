namespace Core.DistributedCache.Abstractions;

/// <summary>
/// Provides access to cache services and storage providers registered
/// within the distributed cache infrastructure.
/// </summary>
/// <remarks>
/// This factory allows applications to retrieve either the default
/// <see cref="ICoreCacheService"/> or a specific cache storage
/// implementation based on the selected <see cref="CacheProviderType"/>.
/// </remarks>
public interface ICacheServiceFactory
{
    /// <summary>
    /// Gets the default cache service configured for the application.
    /// </summary>
    /// <returns>
    /// The default <see cref="ICoreCacheService"/> instance.
    /// </returns>
    ICoreCacheService GetDefaultCache();

    /// <summary>
    /// Gets the cache storage implementation associated with the specified provider.
    /// </summary>
    /// <param name="type">
    /// The cache provider to retrieve.
    /// </param>
    /// <returns>
    /// An <see cref="ICacheStorage"/> implementation for the specified provider.
    /// </returns>
    /// <exception cref="NotSupportedException">
    /// Thrown when the specified cache provider is not supported.
    /// </exception>
    ICacheStorage GetStorage(CacheProviderType type);
}
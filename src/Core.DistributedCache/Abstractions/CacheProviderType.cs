
namespace Core.DistributedCache.Abstractions;

/// <summary>
/// Specifies the cache provider used to store and retrieve cached entries.
/// </summary>
public enum CacheProviderType
{
    /// <summary>
    /// Stores cache entries in the application's memory.
    /// </summary>
    Memory,

    /// <summary>
    /// Stores cache entries in a Redis server.
    /// </summary>
    Redis
}
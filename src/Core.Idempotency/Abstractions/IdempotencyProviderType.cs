namespace Core.Idempotency.Abstractions;

/// <summary>
/// Specifies the cache provider used to store and retrieve cached entries.
/// </summary>
public enum IdempotencyProviderType
{
    /// <summary>
    /// Stores cache entries in a Redis server.
    /// </summary>
    Redis,

    /// <summary>
    /// Stores cache entries in a PostgreSQL database.
    /// </summary>
    PostgreSQL
}
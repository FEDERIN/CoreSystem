using Core.Cache.Abstractions;
using Core.Serialization;
using StackExchange.Redis;

namespace Core.Cache.Options;

/// <summary>
/// Represents the configuration options for the distributed cache.
/// </summary>
/// <remarks>
/// These options control the default cache provider, serialization format,
/// expiration behavior, Redis connectivity, resilience policies,
/// and cache rehydration settings.
/// </remarks>
public class CacheOptions
{
    /// <summary>
    /// Gets or sets the default cache provider used by the library.
    /// </summary>
    /// <remarks>
    /// If not specified, the library selects the appropriate provider
    /// based on the registered services.
    /// </remarks>
    public CacheProviderType DefaultProvider { get; set; }
        = CacheProviderType.Memory;

    /// <summary>
    /// Gets or sets an optional prefix applied to all generated cache keys.
    /// </summary>
    /// <remarks>
    /// Using an instance name allows multiple applications or environments
    /// to safely share the same Redis instance without key collisions.
    /// </remarks>
    public string? InstanceName { get; set; }

    /// <summary>
    /// Gets or sets the default expiration applied to cache entries
    /// when no explicit expiration is provided.
    /// </summary>
    public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromMinutes(30);

    /// <summary>
    /// Gets or sets the maximum allowed size, in bytes, for a cache entry.
    /// </summary>
    /// <remarks>
    /// Cache entries exceeding this limit may be ignored or rejected,
    /// depending on the configured cache provider.
    /// </remarks>
    public long MaxCacheableSize { get; set; } = 1024 * 1024;

    /// <summary>
    /// Gets or sets the Redis-specific configuration.
    /// </summary>
    public RedisOptions Redis { get; set; } = new();

    /// <summary>
    /// Gets or sets the serializer used to store cache entries.
    /// </summary>
    /// <remarks>
    /// Supported serializers include JSON, MessagePack, and Protocol Buffers.
    /// </remarks>
    public SerializerType SerializerType { get; set; } = SerializerType.Json;

    /// <summary>
    /// Gets or sets the interval between cache rehydration cycles.
    /// </summary>
    /// <remarks>
    /// Cache rehydration attempts to restore entries that were temporarily
    /// stored in the fallback provider after the primary provider becomes
    /// available again.
    /// </remarks>
    public TimeSpan RehydrationInterval { get; set; }
        = TimeSpan.FromSeconds(30);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    public void CopyFrom(CacheOptions source)
    {
        ArgumentNullException.ThrowIfNull(source);

        DefaultProvider = source.DefaultProvider;
        InstanceName = source.InstanceName;
        DefaultExpiration = source.DefaultExpiration;
        MaxCacheableSize = source.MaxCacheableSize;
        SerializerType = source.SerializerType;
        RehydrationInterval = source.RehydrationInterval;

        Redis = new RedisOptions
        {
            Enabled = source.Redis.Enabled,
            Connection = source.Redis.Connection,
            Configuration = source.Redis.Configuration
        };
    }
}

/// <summary>
/// Represents the configuration settings for the Redis cache provider.
/// </summary>
public class RedisOptions
{
    /// <summary>
    /// Gets or sets the delegate used to configure the Redis connection.
    /// </summary>
    /// <remarks>
    /// The provided <see cref="ConfigurationOptions"/> instance is used to
    /// configure the underlying StackExchange.Redis connection.
    /// </remarks>
    public Action<ConfigurationOptions>? Configuration { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the Redis provider is enabled.
    /// </summary>
    /// <remarks>
    /// When disabled, the library uses the in-memory cache provider only.
    /// </remarks>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the name of the configuration section used to configure Redis.
    /// </summary>
    public string? Connection { get; set; }
}
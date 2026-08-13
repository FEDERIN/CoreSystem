using Core.Serialization;

namespace Core.Cache.Options;

/// <summary>
/// Represents the configuration options for the cache.
/// </summary>
/// <remarks>
/// These options control cache behavior shared across the core cache
/// orchestration, including key naming, serialization, expiration,
/// entry size limits, and rehydration settings.
/// </remarks>
public class CacheOptions
{
    /// <summary>
    /// Master switch to enable or disable the cache logic.
    /// Default is false.
    /// </summary>
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// Gets or sets an optional prefix applied to generated cache keys.
    /// </summary>
    /// <remarks>
    /// Using an instance name allows multiple applications or environments
    /// to safely share the same cache infrastructure without key collisions.
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
    /// Gets or sets the serializer used to store cache entries.
    /// </summary>
    /// <remarks>
    /// Supported serializers include JSON, MessagePack, and Protocol Buffers.
    /// </remarks>
    public SerializerType SerializerType { get; set; } = SerializerType.Json;

    ///// <summary>
    ///// Gets or sets the interval between cache rehydration cycles.
    ///// </summary>
    ///// <remarks>
    ///// Cache rehydration attempts to restore entries that were temporarily
    ///// stored in the fallback provider after the primary provider becomes
    ///// available again.
    ///// </remarks>
    //public TimeSpan RehydrationInterval { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    public void CopyFrom(CacheOptions source)
    {
        ArgumentNullException.ThrowIfNull(source);
        Enabled = source.Enabled;
        InstanceName = source.InstanceName;
        DefaultExpiration = source.DefaultExpiration;
        MaxCacheableSize = source.MaxCacheableSize;
        SerializerType = source.SerializerType;
    }
}
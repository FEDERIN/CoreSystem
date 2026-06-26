using StackExchange.Redis;

namespace Core.DistributedCache.Options;

public class CacheOptions
{
    public string? DefaultProvider { get; set; }
    public string? InstanceName { get; set; }
    public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromMinutes(30);
    public long MaxCacheableSize { get; set; } = 1024 * 1024;
    public RedisOptions Redis { get; set; } = new();
    public SerializerType SerializerType { get; set; } = SerializerType.Json;
}

public class RedisOptions
{
    public Action<ConfigurationOptions>? Configuration { get; set; }
    public bool Enabled { get; set; } = true;
}

public enum SerializerType : byte
{
    Json = 1,
    MessagePack = 2,
    Protobuf = 3
}
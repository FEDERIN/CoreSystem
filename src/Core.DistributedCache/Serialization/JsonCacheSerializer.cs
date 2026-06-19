using System.Text.Json;
using Core.DistributedCache.Abstractions;

namespace Core.DistributedCache.Serialization;

public class JsonCacheSerializer : ICacheSerializer
{
    public string Serialize<T>(T value) => JsonSerializer.Serialize(value);
    public T? Deserialize<T>(string value) => JsonSerializer.Deserialize<T>(value);
}
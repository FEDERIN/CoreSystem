using Core.DistributedCache.Abstractions;
using System.Text.Json;

namespace Core.DistributedCache.Serialization;

internal sealed class JsonCacheSerializer : ICacheSerializer
{
    private readonly JsonSerializerOptions _options;

    public JsonCacheSerializer(JsonSerializerOptions? options = null)
    {
        _options = options ?? new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    public byte[] Serialize<T>(T value)
    {
        return JsonSerializer.SerializeToUtf8Bytes(value, _options);
    }

    public T? Deserialize<T>(byte[] bytes)
    {
        if (bytes == null || bytes.Length == 0)
            return default;

        return JsonSerializer.Deserialize<T>(bytes, _options);
    }
}
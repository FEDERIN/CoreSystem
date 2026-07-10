using Core.Serialization.Abstractions;
using System.Text.Json;

namespace Core.Serialization.Json;

internal sealed class JsonPayloadSerializer(
    JsonSerializerOptions options) : ISerializer
{
    private readonly JsonSerializerOptions _options = options;

    public byte[] Serialize<T>(T value)
    {
        try
        {
            return JsonSerializer.SerializeToUtf8Bytes(value, _options);
        }
        catch (Exception ex)
        {
            throw new CoreSerializationException(
                SerializerType.Json,
                "Unable to serialize using JSON.",
                ex);
        }
    }

    public T? Deserialize<T>(byte[] bytes)
    {
        if (bytes.Length == 0)
            return default;

        try
        {
            return JsonSerializer.Deserialize<T>(bytes, _options);
        }
        catch (Exception ex)
        {
            throw new CoreSerializationException(
                SerializerType.Json,
                "Unable to deserialize cache entry using JSON.",
                ex);
        }
    }
}

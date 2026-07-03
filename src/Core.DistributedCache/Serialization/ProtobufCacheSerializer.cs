using Core.DistributedCache.Abstractions;
using ProtoBuf;

namespace Core.DistributedCache.Serialization;

internal sealed class ProtobufCacheSerializer : ICacheSerializer
{
    public bool RequiresHeader => true;

    public byte[] Serialize<T>(T value)
    {
        using var stream = new MemoryStream();
        Serializer.Serialize(stream, value);
        return stream.ToArray();
    }

    public T? Deserialize<T>(byte[] bytes)
    {
        if (bytes == null || bytes.Length == 0) return default;
        using var stream = new MemoryStream(bytes);
        return Serializer.Deserialize<T>(stream);
    }
}
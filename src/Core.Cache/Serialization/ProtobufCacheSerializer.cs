using Core.Cache.Abstractions;
using Core.Cache.Exceptions;
using Core.Cache.Options;
using ProtoBuf;

namespace Core.Cache.Serialization;

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
        if (bytes == null || bytes.Length == 0)
            return default;

        try
        {
            using var stream = new MemoryStream(bytes);

            return Serializer.Deserialize<T>(stream);
        }
        catch (Exception ex)
        {
            throw new CacheDeserializationException(
                SerializerType.Protobuf,
                "Unable to deserialize cache entry using Protocol Buffers.",
                ex);
        }
    }
}
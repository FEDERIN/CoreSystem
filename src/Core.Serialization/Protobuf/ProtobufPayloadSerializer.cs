using Core.Serialization.Abstractions;
using ProtoBuf;

namespace Core.Serialization.Protobuf;

internal sealed class ProtobufPayloadSerializer : ISerializer
{
    public byte[] Serialize<T>(T value)
    {
        try
        {
            using var stream = new MemoryStream();
            Serializer.Serialize(stream, value);
            return stream.ToArray();
        }
        catch (Exception ex)
        {
            throw new CoreSerializationException(
                SerializerType.Protobuf,
                "Unable to serialize object using Protocol Buffers.",
                ex);
        }
    }

    public T? Deserialize<T>(byte[] bytes)
    {
        if (bytes.Length == 0)
            return default;

        try
        {
            using var stream = new MemoryStream(bytes);

            return Serializer.Deserialize<T>(stream);
        }
        catch (Exception ex)
        {
            throw new CoreSerializationException(
                SerializerType.Protobuf,
                "Unable to deserialize cache entry using Protocol Buffers.",
                ex);
        }
    }
}

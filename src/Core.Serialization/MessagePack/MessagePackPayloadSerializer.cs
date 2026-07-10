using Core.Serialization.Abstractions;
using MessagePack;

namespace Core.Serialization.MessagePack;

internal sealed class MessagePackPayloadSerializer(
    MessagePackSerializerOptions options)
        : ISerializer
{
    private readonly MessagePackSerializerOptions _options = options;

    public byte[] Serialize<T>(T value)
    {
        try
        {
            return MessagePackSerializer.Serialize(value, _options);
        }
        catch (Exception ex)
        {
            throw new CoreSerializationException(
                SerializerType.MessagePack,
                "Unable to serialize object using MessagePack.",
                ex);
        }
    }

    public T? Deserialize<T>(byte[] bytes)
    {
        if (bytes.Length == 0)
            return default;

        try
        {
            return MessagePackSerializer.Deserialize<T>(
                bytes,
                _options);
        }
        catch (Exception ex)
        {
            throw new CoreSerializationException(
                SerializerType.MessagePack,
                "Unable to deserialize object using MessagePack.",
                ex);
        }
    }
}
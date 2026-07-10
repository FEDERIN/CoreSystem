using Core.Serialization.Abstractions;
using Core.Serialization.Json;
using Core.Serialization.MessagePack;
using Core.Serialization.Protobuf;

namespace Core.Serialization;

internal sealed class SerializerFactory(
    JsonPayloadSerializer json,
    MessagePackPayloadSerializer messagePack,
    ProtobufPayloadSerializer protobuf)
    : ISerializerFactory
{
    public ISerializer GetSerializer(SerializerType type)
        => type switch
        {
            SerializerType.Json => json,
            SerializerType.MessagePack => messagePack,
            SerializerType.Protobuf => protobuf,
            _ => throw new NotSupportedException(
                $"Serializer '{type}' is not supported.")
        };
}
using Core.Serialization.Abstractions;

namespace Core.Serialization;

internal sealed class PayloadSerializer(
    SerializationOptions options,
    ISerializerFactory serializerFactory)
    : IPayloadSerializer
{
    private readonly ISerializer _serializer =
        serializerFactory.GetSerializer(
            options.DefaultSerializer);

    public byte[] Serialize<T>(T value)
        => _serializer.Serialize(value);

    public T? Deserialize<T>(ReadOnlyMemory<byte> payload)
    {
        if (payload.IsEmpty)
            return default;

        return _serializer.Deserialize<T>(payload.ToArray());
    }
}
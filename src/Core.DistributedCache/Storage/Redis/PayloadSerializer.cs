using Core.DistributedCache.Abstractions;
using Core.DistributedCache.Options;
using Core.DistributedCache.Storage.Abstractions;

namespace Core.DistributedCache.Storage.Redis;

internal sealed class PayloadSerializer(
    CacheOptions options,
    ICacheSerializerFactory serializerFactory)
    : IPayloadSerializer
{
    private readonly CacheOptions _options = options;
    private readonly ICacheSerializerFactory _factory = serializerFactory;

    public byte[] Serialize<T>(T value)
    {
        var serializer =
            _factory.GetSerializer(_options.SerializerType);

        var payload = serializer.Serialize(value);

        if (!serializer.RequiresHeader)
            return payload;

        var result = new byte[payload.Length + 1];

        result[0] = (byte)_options.SerializerType;

        payload.CopyTo(result.AsSpan(1));

        return result;
    }

    public T? Deserialize<T>(ReadOnlyMemory<byte> payload)
    {
        if (payload.IsEmpty)
            return default;

        var span = payload.Span;

        if (Enum.IsDefined(typeof(SerializerType), span[0]))
        {
            var serializer =
                _factory.GetSerializer((SerializerType)span[0]);

            return serializer.Deserialize<T>(
                span[1..].ToArray());
        }

        return _factory
            .GetSerializer(SerializerType.Json)
            .Deserialize<T>(span.ToArray());
    }
}
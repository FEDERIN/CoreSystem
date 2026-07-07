using Core.Cache.Abstractions;
using Core.Cache.Exceptions;
using Core.Cache.Options;
using MessagePack;
using MessagePack.Resolvers;

namespace Core.Cache.Serialization;

internal sealed class MessagePackCacheSerializer : ICacheSerializer
{
    private readonly MessagePackSerializerOptions _options =
        MessagePackSerializerOptions.Standard.WithResolver(ContractlessStandardResolver.Instance);

    public bool RequiresHeader => true;

    public byte[] Serialize<T>(T value) => MessagePackSerializer.Serialize(value, _options);

    public T? Deserialize<T>(byte[] bytes)
    {
        try
        {
            return MessagePackSerializer.Deserialize<T>(bytes, _options);
        }
        catch (Exception ex)
        {
            throw new CacheDeserializationException(
                SerializerType.MessagePack,
                "Unable to deserialize cache entry using MessagePack.",
                ex);
        }
    }
}
using Core.Cache.Abstractions;
using Core.Cache.Options;
using Core.Cache.Serialization;

namespace Core.Cache.Storage;

internal sealed class CacheSerializerFactory(
    JsonCacheSerializer json,
    MessagePackCacheSerializer msgPack,
    ProtobufCacheSerializer proto) : ICacheSerializerFactory
{
    private readonly JsonCacheSerializer _json = json;
    private readonly MessagePackCacheSerializer _msgPack = msgPack;
    private readonly ProtobufCacheSerializer _proto = proto;

    public ICacheSerializer GetSerializer(SerializerType type) => type switch
    {
        SerializerType.MessagePack => _msgPack,
        SerializerType.Protobuf => _proto,
        _ => _json
    };
}
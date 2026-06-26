using Core.DistributedCache.Abstractions;
using Core.DistributedCache.Options;
using Core.DistributedCache.Serialization;

namespace Core.DistributedCache.Storage;

internal class CacheSerializerFactory(
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
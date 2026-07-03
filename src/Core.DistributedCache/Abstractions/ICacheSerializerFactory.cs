using Core.DistributedCache.Options;

namespace Core.DistributedCache.Abstractions;

internal interface ICacheSerializerFactory
{
    ICacheSerializer GetSerializer(SerializerType type);
}
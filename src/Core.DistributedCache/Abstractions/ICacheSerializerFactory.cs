using Core.DistributedCache.Options;

namespace Core.DistributedCache.Abstractions;

public interface ICacheSerializerFactory
{
    ICacheSerializer GetSerializer(SerializerType type);
}
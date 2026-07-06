using Core.Cache.Options;

namespace Core.Cache.Abstractions;

internal interface ICacheSerializerFactory
{
    ICacheSerializer GetSerializer(SerializerType type);
}
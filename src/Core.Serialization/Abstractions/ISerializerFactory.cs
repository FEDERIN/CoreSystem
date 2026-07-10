namespace Core.Serialization.Abstractions;

internal interface ISerializerFactory
{
    ISerializer GetSerializer(SerializerType type);
}
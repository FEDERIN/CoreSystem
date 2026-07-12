using Core.Serialization;

namespace Core.Cache.Exceptions;

internal sealed class CacheDeserializationException(
    SerializerType serializer,
    string message,
    Exception innerException) : Exception(message, innerException)
{
    public SerializerType Serializer { get; } = serializer;
}
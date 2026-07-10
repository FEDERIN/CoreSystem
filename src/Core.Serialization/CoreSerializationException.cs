namespace Core.Serialization;

/// <summary>
/// Represents an error that occurs during serialization or deserialization.
/// </summary>
/// <remarks>
/// This exception wraps the original serializer-specific exception while
/// exposing the serializer that produced the failure.
/// </remarks>
public sealed class CoreSerializationException(
    SerializerType serializer,
    string message,
    Exception innerException)
    : Exception(message, innerException)
{
    /// <summary>
    /// Gets the serializer that caused the exception.
    /// </summary>
    public SerializerType Serializer { get; } = serializer;
}
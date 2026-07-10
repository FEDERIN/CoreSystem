namespace Core.Serialization.Abstractions;

/// <summary>
/// Defines a serializer capable of converting objects to and from a binary representation.
/// </summary>
/// <remarks>
/// Implementations encapsulate the underlying serialization technology,
/// allowing consumers to work with a consistent API regardless of the
/// serialization format (JSON, MessagePack, Protocol Buffers, etc.).
/// </remarks>
public interface ISerializer
{
    /// <summary>
    /// Serializes the specified value into a byte array.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the object to serialize.
    /// </typeparam>
    /// <param name="value">
    /// The object to serialize.
    /// </param>
    /// <returns>
    /// A byte array containing the serialized representation of the object.
    /// </returns>
    /// <exception cref="CoreSerializationException">
    /// Thrown when the serialization operation fails.
    /// </exception>
    byte[] Serialize<T>(T value);

    /// <summary>
    /// Deserializes the specified byte array into an object.
    /// </summary>
    /// <typeparam name="T">
    /// The expected type of the deserialized object.
    /// </typeparam>
    /// <param name="bytes">
    /// The serialized binary payload.
    /// </param>
    /// <returns>
    /// The deserialized object, or <see langword="default"/> if the payload
    /// is empty and the serializer supports empty values.
    /// </returns>
    /// <exception cref="CoreSerializationException">
    /// Thrown when the deserialization operation fails.
    /// </exception>
    T? Deserialize<T>(byte[] bytes);
}
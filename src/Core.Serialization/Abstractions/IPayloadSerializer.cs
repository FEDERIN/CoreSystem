namespace Core.Serialization.Abstractions;

/// <summary>
/// Provides a high-level abstraction for serializing and deserializing
/// application payloads using the serializer configured for the library.
/// </summary>
/// <remarks>
/// Unlike <see cref="ISerializer"/>, which represents a specific serialization
/// implementation (JSON, MessagePack, Protocol Buffers, etc.),
/// <see cref="IPayloadSerializer"/> automatically resolves the configured
/// serializer through <see cref="SerializationOptions.DefaultSerializer"/>.
///
/// This abstraction allows infrastructure components to serialize and
/// deserialize payloads without depending on a specific serialization format.
/// Typical consumers include distributed cache providers, idempotency storage,
/// messaging systems, and other infrastructure libraries.
/// </remarks>
public interface IPayloadSerializer
{
    /// <summary>
    /// Serializes the specified value using the configured serializer.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the object to serialize.
    /// </typeparam>
    /// <param name="value">
    /// The object to serialize.
    /// </param>
    /// <returns>
    /// A byte array containing the serialized payload.
    /// </returns>
    byte[] Serialize<T>(T value);

    /// <summary>
    /// Deserializes the specified payload using the configured serializer.
    /// </summary>
    /// <typeparam name="T">
    /// The expected type of the deserialized object.
    /// </typeparam>
    /// <param name="payload">
    /// The serialized payload.
    /// </param>
    /// <returns>
    /// The deserialized object, or <see langword="default"/> if the payload
    /// is empty.
    /// </returns>
    T? Deserialize<T>(ReadOnlyMemory<byte> payload);
}
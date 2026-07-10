namespace Core.Serialization;

/// <summary>
/// Specifies the serialization format used to convert objects to and from
/// binary data.
/// </summary>
public enum SerializerType : byte
{
    /// <summary>
    /// Uses <see cref="System.Text.Json.JsonSerializer"/>.
    /// </summary>
    Json = 1,

    /// <summary>
    /// Uses MessagePack binary serialization.
    /// </summary>
    MessagePack = 2,

    /// <summary>
    /// Uses Protocol Buffers binary serialization.
    /// </summary>
    Protobuf = 3
}
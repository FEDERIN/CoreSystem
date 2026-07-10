using MessagePack;
using MessagePack.Resolvers;
using System.Text.Json;

namespace Core.Serialization;

/// <summary>
/// Represents the configuration options used by the Core.Serialization library.
/// </summary>
/// <remarks>
/// These options allow customization of the underlying serializers while
/// preserving a unified serialization abstraction.
/// </remarks>
public sealed class SerializationOptions
{
    /// <summary>
    /// Gets or sets the default serializer type used for serialization and deserialization.
    /// </summary>
    /// <remarks>
    /// The default value is <see cref="SerializerType.Json"/>.
    /// </remarks>
    public SerializerType DefaultSerializer { get; set; }
    = SerializerType.Json;

    /// <summary>
    /// Gets or sets the configuration used by the JSON serializer.
    /// </summary>
    /// <remarks>
    /// By default, property names are serialized using camel case and
    /// deserialization is case-insensitive.
    /// </remarks>
    public JsonSerializerOptions JsonOptions { get; set; } =
        new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

    /// <summary>
    /// Gets or sets the configuration used by the MessagePack serializer.
    /// </summary>
    /// <remarks>
    /// The default configuration uses the
    /// <see cref="ContractlessStandardResolver"/> resolver.
    /// </remarks>
    public MessagePackSerializerOptions MessagePackOptions { get; set; } =
        MessagePackSerializerOptions.Standard
            .WithResolver(ContractlessStandardResolver.Instance);
}
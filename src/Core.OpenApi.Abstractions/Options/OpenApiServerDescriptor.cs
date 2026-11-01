namespace Core.OpenApi.Options;

/// <summary>
/// Describes one server listed in the OpenAPI document.
/// </summary>
public sealed class OpenApiServerDescriptor
{
    /// <summary>
    /// Gets or initializes the absolute base URL of the server.
    /// </summary>
    /// <value>
    /// The URL rendered in the <c>servers</c> section, for example
    /// <c>https://api.example.com</c>.
    /// </value>
    public required string Url { get; init; }

    /// <summary>
    /// Gets or initializes the display name of the server.
    /// </summary>
    /// <value>
    /// The label shown in Swagger UI. The default value is <see langword="null"/>.
    /// </value>
    public string? Description { get; init; }
}

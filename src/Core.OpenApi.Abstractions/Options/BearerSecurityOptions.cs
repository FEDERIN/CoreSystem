namespace Core.OpenApi.Options;

/// <summary>
/// Describes the bearer security scheme registered in the OpenAPI document.
/// </summary>
public sealed class BearerSecurityOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether the security scheme is registered.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to register the scheme and apply it to every operation;
    /// otherwise, <see langword="false"/>. The default value is <see langword="true"/>.
    /// </value>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the identifier of the security scheme.
    /// </summary>
    /// <value>
    /// The component key referenced by the security requirement.
    /// The default value is <c>bearer</c>.
    /// </value>
    public string SchemeName { get; set; } = "bearer";

    /// <summary>
    /// Gets or sets the header that carries the token.
    /// </summary>
    /// <value>
    /// The header name. The default value is <c>Authorization</c>.
    /// </value>
    public string HeaderName { get; set; } = "Authorization";

    /// <summary>
    /// Gets or sets the HTTP authentication scheme.
    /// </summary>
    /// <value>
    /// The scheme name reported in the document. The default value is <c>bearer</c>.
    /// </value>
    public string Scheme { get; set; } = "bearer";

    /// <summary>
    /// Gets or sets the bearer format hint.
    /// </summary>
    /// <value>
    /// The token format reported in the document. The default value is <c>JWT</c>.
    /// </value>
    public string? BearerFormat { get; set; } = "JWT";

    /// <summary>
    /// Gets or sets the scheme description.
    /// </summary>
    /// <value>
    /// The help text rendered by Swagger UI. The default value is
    /// <c>JWT Authorization header. Example: Bearer {token}</c>.
    /// </value>
    public string Description { get; set; } = "JWT Authorization header. Example: Bearer {token}";
}

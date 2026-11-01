using Microsoft.AspNetCore.Builder;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Core.OpenApi.Options;

/// <summary>
/// Provides configuration options for CoreSystem-generated OpenAPI documents.
/// </summary>
/// <remarks>
/// The options are evaluated once, when <c>AddCoreOpenApi</c> is called. Later mutations
/// are visible to the endpoint that serves the patched document but not to the document
/// that the generator already produced.
/// </remarks>
public sealed class CoreOpenApiOptions
{
    /// <summary>
    /// Gets or sets the name of the generated OpenAPI document.
    /// </summary>
    /// <value>
    /// The document name used by Swagger generation and by Swagger UI.
    /// The default value is <c>v1</c>.
    /// </value>
    public string DocumentName { get; set; } = "v1";

    /// <summary>
    /// Gets or sets the title displayed by Swagger UI.
    /// </summary>
    /// <value>
    /// The API title. When the value is <see langword="null"/> or white space, the name of
    /// the entry assembly is used.
    /// </value>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the API description.
    /// </summary>
    /// <value>
    /// An optional description rendered below the title. The default value is
    /// <see langword="null"/>.
    /// </value>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the document version.
    /// </summary>
    /// <value>
    /// The value reported in the <c>info.version</c> field.
    /// The default value is <c>v1</c>.
    /// </value>
    public string Version { get; set; } = "v1";

    /// <summary>
    /// Gets or sets the route prefix that serves Swagger UI.
    /// </summary>
    /// <value>
    /// The UI route prefix without a leading slash. The default value is <c>swagger</c>.
    /// </value>
    public string SwaggerRoutePrefix { get; set; } = "swagger";

    /// <summary>
    /// Gets or sets the route that serves the patched OpenAPI document.
    /// </summary>
    /// <value>
    /// The absolute route of the JSON document. When the value is <see langword="null"/>,
    /// the route is composed from <see cref="SwaggerRoutePrefix"/>, <see cref="DocumentName"/>
    /// and <c>swagger.json</c>. The default value is <see langword="null"/>.
    /// </value>
    public string? JsonEndpointRoute { get; set; }

    /// <summary>
    /// Gets or sets the route template of the unpatched document produced by Swashbuckle.
    /// </summary>
    /// <value>
    /// The route template used to expose the generator output for diagnostics.
    /// When the value is <see langword="null"/>, the template is composed from
    /// <see cref="SwaggerRoutePrefix"/> with an <c>-original</c> suffix. Set the value to an
    /// empty string to stop exposing the unpatched document. The default value is
    /// <see langword="null"/>.
    /// </value>
    public string? OriginalJsonRouteTemplate { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the document is post-processed by
    /// <see cref="OpenApiDocumentPatcher"/> before it is served.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to serve the patched document; otherwise, <see langword="false"/>.
    /// The default value is <see langword="true"/>.
    /// </value>
    public bool EnableDocumentPatch { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the patched document is cached in memory.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to serialize and patch the document once per application
    /// lifetime; otherwise, <see langword="false"/>. The default value is
    /// <see langword="true"/>.
    /// </value>
    public bool CachePatchedDocument { get; set; } = true;

    /// <summary>
    /// Gets the bearer authentication configuration.
    /// </summary>
    /// <value>
    /// The security scheme registered in the document. Set
    /// <see cref="BearerSecurityOptions.Enabled"/> to <see langword="false"/> to omit it.
    /// </value>
    public BearerSecurityOptions Bearer { get; } = new();

    /// <summary>
    /// Gets the servers listed in the document.
    /// </summary>
    /// <value>
    /// The server entries rendered by Swagger UI. The collection is empty by default.
    /// </value>
    public IList<OpenApiServerDescriptor> Servers { get; } = [];

    /// <summary>
    /// Gets the XML documentation files included in the document.
    /// </summary>
    /// <value>
    /// Absolute or relative XML documentation paths. When the collection is empty, every
    /// <c>*.xml</c> file in the application base directory is included.
    /// </value>
    public IList<string> XmlCommentFiles { get; } = [];

    /// <summary>
    /// Gets or sets a value indicating whether controller XML comments are included.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to include controller summaries and remarks; otherwise,
    /// <see langword="false"/>. The default value is <see langword="true"/>.
    /// </value>
    public bool IncludeControllerXmlComments { get; set; } = true;

    /// <summary>
    /// Gets the configuration of the shared problem-details responses.
    /// </summary>
    /// <value>
    /// The response catalog used by <see cref="OpenApiDocumentPatcher"/>.
    /// </value>
    public OpenApiProblemResponsesOptions ProblemResponses { get; } = new();

    /// <summary>
    /// Gets or sets an action that configures additional Swagger generation features.
    /// </summary>
    /// <value>
    /// A callback invoked after the CoreSystem filters are registered. Use it to enable
    /// annotations or example filters supplied by other packages. The default value is
    /// <see langword="null"/>.
    /// </value>
    public Action<SwaggerGenOptions>? ConfigureSwaggerGen { get; set; }

    /// <summary>
    /// Gets or sets an action that configures the endpoint serving the patched document.
    /// </summary>
    /// <value>
    /// A callback invoked for the JSON endpoint before it is mapped. Use it to exclude the
    /// endpoint from rate limiting or from the document itself. The default value is
    /// <see langword="null"/>.
    /// </value>
    public Action<IEndpointConventionBuilder>? ConfigureJsonEndpoint { get; set; }

    /// <summary>
    /// Validates the configuration that is required to generate a document.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Thrown when a required value is empty or when a response definition misses a
    /// status code, a component name or an error code.
    /// </exception>
    internal void Validate()
    {
        if (string.IsNullOrWhiteSpace(DocumentName))
        {
            throw new ArgumentException(
                "OpenAPI document name is required.",
                nameof(DocumentName));
        }

        if (string.IsNullOrWhiteSpace(SwaggerRoutePrefix))
        {
            throw new ArgumentException(
                "Swagger UI route prefix is required.",
                nameof(SwaggerRoutePrefix));
        }

        if (string.IsNullOrWhiteSpace(Version))
        {
            throw new ArgumentException(
                "OpenAPI document version is required.",
                nameof(Version));
        }

        foreach (var definition in ProblemResponses.Definitions)
        {
            if (string.IsNullOrWhiteSpace(definition.StatusCode))
            {
                throw new ArgumentException(
                    "Every problem response requires a status code.",
                    nameof(ProblemResponses));
            }

            if (string.IsNullOrWhiteSpace(definition.ComponentName))
            {
                throw new ArgumentException(
                    "Every problem response requires a component name.",
                    nameof(ProblemResponses));
            }

            if (string.IsNullOrWhiteSpace(definition.ErrorCode))
            {
                throw new ArgumentException(
                    "Every problem response requires an error code.",
                    nameof(ProblemResponses));
            }
        }
    }

    /// <summary>
    /// Gets the absolute route that serves the patched document.
    /// </summary>
    internal string ResolvedJsonEndpointRoute =>
        JsonEndpointRoute
        ?? $"/{SwaggerRoutePrefix.TrimEnd('/')}/{DocumentName}/swagger.json";

    /// <summary>
    /// Gets the route template of the unpatched document.
    /// </summary>
    internal string ResolvedOriginalJsonRouteTemplate =>
        OriginalJsonRouteTemplate
        ?? $"{SwaggerRoutePrefix.TrimEnd('/')}-original/{{documentName}}/swagger.json";
}

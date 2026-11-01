namespace Core.OpenApi.Options;

/// <summary>
/// Configures the shared problem-details responses produced by
/// <see cref="OpenApiDocumentPatcher"/>.
/// </summary>
public sealed class OpenApiProblemResponsesOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenApiProblemResponsesOptions"/> class
    /// with a generic catalog of RFC 9457 responses.
    /// </summary>
    public OpenApiProblemResponsesOptions()
    {
        Definitions = CreateDefaultDefinitions();
    }

    /// <summary>
    /// Gets or sets a value indicating whether the responses are registered.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to create the components and rewrite operations; otherwise,
    /// <see langword="false"/>. The default value is <see langword="true"/>.
    /// </value>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the default schema identifier referenced by the responses.
    /// </summary>
    /// <value>
    /// The schema component name used when a definition does not declare its own.
    /// The default value is <c>ProblemDetails</c>.
    /// </value>
    public string SchemaId { get; set; } = "ProblemDetails";

    /// <summary>
    /// Gets or sets the media type used by the responses.
    /// </summary>
    /// <value>
    /// The content type key of every generated response.
    /// The default value is <c>application/problem+json</c>.
    /// </value>
    public string ContentType { get; set; } = "application/problem+json";

    /// <summary>
    /// Gets or sets the prefix composed with <see cref="ProblemResponseDefinition.ErrorCode"/>
    /// to build the <c>type</c> field of the example payload.
    /// </summary>
    /// <value>
    /// The URI prefix, for example <c>urn:example:error:</c>. When the value is
    /// <see langword="null"/>, the <c>type</c> field is omitted. The default value is
    /// <see langword="null"/>.
    /// </value>
    public string? TypePrefix { get; set; }

    /// <summary>
    /// Gets or sets the trace identifier rendered in the example payload.
    /// </summary>
    /// <value>
    /// The value of the <c>traceId</c> extension. When the value is <see langword="null"/>,
    /// the field is omitted. The default value is <see langword="null"/>.
    /// </value>
    public string? TraceIdExample { get; set; }

    /// <summary>
    /// Gets the schema identifiers considered problem schemas.
    /// </summary>
    /// <value>
    /// The schema names used by
    /// <see cref="ProblemResponseReplacementMode.WhenContentMatchesProblemDetails"/> to
    /// decide whether an existing response already describes a problem. The collection
    /// contains <c>ProblemDetails</c> and <c>ValidationProblemDetails</c> by default.
    /// </value>
    public IList<string> ProblemSchemaIds { get; } =
        new List<string> { "ProblemDetails", "ValidationProblemDetails" };

    /// <summary>
    /// Gets the response catalog.
    /// </summary>
    /// <value>
    /// The definitions registered as components and applied to every operation.
    /// </value>
    public IList<ProblemResponseDefinition> Definitions { get; }

    /// <summary>
    /// Creates the generic catalog used when the consumer does not customize the list.
    /// </summary>
    /// <returns>
    /// A mutable list of response definitions ordered by status code.
    /// </returns>
    private static List<ProblemResponseDefinition> CreateDefaultDefinitions()
    {
        var validation = new ProblemResponseDefinition
        {
            StatusCode = "400",
            ComponentName = "ValidationError",
            ErrorCode = "VALIDATION_ERROR",
            Title = "Validation error",
            Description = "Validation error",
            Detail = "One or more validation errors occurred.",
            SchemaId = "ValidationProblemDetails",
            Replacement = ProblemResponseReplacementMode.WhenContentMatchesProblemDetails
        };

        validation.AdditionalExampleFields["errors"] = new Dictionary<string, object?>
        {
            ["name"] = new[] { "The Name field is required." }
        };

        return
        [
            new ProblemResponseDefinition
            {
                StatusCode = "401",
                ComponentName = "Unauthorized",
                ErrorCode = "UNAUTHORIZED",
                Title = "Unauthorized",
                Description = "Unauthorized",
                Detail = "A valid access token is required."
            },
            new ProblemResponseDefinition
            {
                StatusCode = "403",
                ComponentName = "Forbidden",
                ErrorCode = "FORBIDDEN",
                Title = "Forbidden",
                Description = "Forbidden",
                Detail = "You do not have permission to access this resource."
            },
            new ProblemResponseDefinition
            {
                StatusCode = "500",
                ComponentName = "InternalError",
                ErrorCode = "INTERNAL_ERROR",
                Title = "Internal Server Error",
                Description = "Internal Server Error",
                Detail = "An unexpected error occurred."
            },
            new ProblemResponseDefinition
            {
                StatusCode = "404",
                ComponentName = "NotFound",
                ErrorCode = "NOT_FOUND",
                Title = "Not Found",
                Description = "Not Found",
                Detail = "The requested resource was not found."
            },
            new ProblemResponseDefinition
            {
                StatusCode = "409",
                ComponentName = "Conflict",
                ErrorCode = "CONFLICT",
                Title = "Conflict",
                Description = "Conflict",
                Detail = "The request conflicts with the current state of the resource."
            },
            validation,
            new ProblemResponseDefinition
            {
                StatusCode = "429",
                ComponentName = "TooManyRequests",
                ErrorCode = "RATE_LIMIT_EXCEEDED",
                Title = "Too Many Requests",
                Description = "Too Many Requests",
                Detail = "Rate limit exceeded. Please retry later.",
                AddRetryAfterHeader = true
            }
        ];
    }
}

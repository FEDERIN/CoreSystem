namespace Core.OpenApi.Options;

/// <summary>
/// Describes one shared problem-details response registered in the document.
/// </summary>
public sealed class ProblemResponseDefinition
{
    /// <summary>
    /// Gets or sets the HTTP status code of the response.
    /// </summary>
    /// <value>
    /// The status code key used inside an operation <c>responses</c> object, for example
    /// <c>404</c>. The value is required.
    /// </value>
    public string StatusCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the component name of the response.
    /// </summary>
    /// <value>
    /// The key used in <c>components.responses</c>, for example <c>NotFound</c>.
    /// The value is required.
    /// </value>
    public string ComponentName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the business error code exposed in the example payload.
    /// </summary>
    /// <value>
    /// The value of the <c>errorCode</c> extension, for example <c>NOT_FOUND</c>.
    /// The value is required.
    /// </value>
    public string ErrorCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the response description.
    /// </summary>
    /// <value>
    /// The text rendered next to the status code. When the value is <see langword="null"/>,
    /// <see cref="Title"/> is used. The default value is <see langword="null"/>.
    /// </value>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the example title.
    /// </summary>
    /// <value>
    /// The value of the <c>title</c> field in the example payload. The default value is
    /// <see langword="null"/>.
    /// </value>
    public string? Title { get; set; }

    /// <summary>
    /// Gets or sets the example detail.
    /// </summary>
    /// <value>
    /// The value of the <c>detail</c> field in the example payload. The default value is
    /// <see langword="null"/>.
    /// </value>
    public string? Detail { get; set; }

    /// <summary>
    /// Gets or sets the schema referenced by the response.
    /// </summary>
    /// <value>
    /// The schema component identifier. When the value is <see langword="null"/>,
    /// <see cref="OpenApiProblemResponsesOptions.SchemaId"/> is used. The default value is
    /// <see langword="null"/>.
    /// </value>
    public string? SchemaId { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the response is added to operations
    /// that do not declare the status code.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to add the reference to every operation; otherwise,
    /// <see langword="false"/>. The default value is <see langword="false"/>.
    /// </value>
    public bool IncludeWhenMissing { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the response declares a
    /// <c>Retry-After</c> header.
    /// </summary>
    /// <value>
    /// <see langword="true"/> to document the header; otherwise, <see langword="false"/>.
    /// The default value is <see langword="false"/>.
    /// </value>
    public bool AddRetryAfterHeader { get; set; }

    /// <summary>
    /// Gets or sets the replacement behavior applied to existing responses.
    /// </summary>
    /// <value>
    /// The mode evaluated for operations that already declare <see cref="StatusCode"/>.
    /// The default value is <see cref="ProblemResponseReplacementMode.Always"/>.
    /// </value>
    public ProblemResponseReplacementMode Replacement { get; set; } =
        ProblemResponseReplacementMode.Always;

    /// <summary>
    /// Gets the additional fields serialized into the example payload.
    /// </summary>
    /// <value>
    /// Extension fields appended after <c>errorCode</c>. The collection is empty by default.
    /// </value>
    public IDictionary<string, object?> AdditionalExampleFields { get; } =
        new Dictionary<string, object?>();
}

namespace Core.OpenApi.Options;

/// <summary>
/// Defines how an existing response is handled when the patcher processes an operation.
/// </summary>
public enum ProblemResponseReplacementMode
{
    /// <summary>
    /// The existing response is left untouched. The component is still created.
    /// </summary>
    Never,

    /// <summary>
    /// The existing response is replaced only when it already describes a problem response,
    /// that is, when it has no content, uses the configured problem content type, or its
    /// schema references one of the configured problem schemas.
    /// </summary>
    WhenContentMatchesProblemDetails,

    /// <summary>
    /// The existing response is always replaced by a reference to the component.
    /// </summary>
    Always
}

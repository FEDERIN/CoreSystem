using Microsoft.AspNetCore.Http;

namespace Core.Http.ProblemDetails.Options;

/// <summary>Configures additional fields for CoreSystem-generated problem details.</summary>
public sealed class CoreProblemDetailsOptions
{
    /// <summary>Invoked after the standard RFC 9457 fields and CoreSystem extensions are populated.</summary>
    public Action<Microsoft.AspNetCore.Mvc.ProblemDetails, HttpContext, Exception>? CustomizeProblemDetails { get; set; }
}

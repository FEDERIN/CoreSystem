using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Core.OpenApi.Attributes;

/// <summary>
/// Declares a <c>204 No Content</c> response.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class ProducesNoContentAttribute : ProducesResponseTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProducesNoContentAttribute"/> class.
    /// </summary>
    public ProducesNoContentAttribute()
        : base(StatusCodes.Status204NoContent)
    {
    }
}

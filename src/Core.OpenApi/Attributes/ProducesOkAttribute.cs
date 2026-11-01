using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Core.OpenApi.Attributes;

/// <summary>
/// Declares a <c>200 OK</c> response of the specified body type.
/// </summary>
/// <typeparam name="T">The response body type.</typeparam>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class ProducesOkAttribute<T> : ProducesResponseTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProducesOkAttribute{T}"/> class.
    /// </summary>
    public ProducesOkAttribute()
        : base(typeof(T), StatusCodes.Status200OK)
    {
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Core.OpenApi.Attributes;

/// <summary>
/// Declares a <c>201 Created</c> response of the specified body type.
/// </summary>
/// <typeparam name="T">The response body type.</typeparam>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public sealed class ProducesCreatedAttribute<T> : ProducesResponseTypeAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProducesCreatedAttribute{T}"/> class.
    /// </summary>
    public ProducesCreatedAttribute()
        : base(typeof(T), StatusCodes.Status201Created)
    {
    }
}

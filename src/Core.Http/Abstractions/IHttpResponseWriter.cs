using Core.Http.Responses;
using Microsoft.AspNetCore.Http;

namespace Core.Http.Abstractions;

/// <summary>
/// Defines a contract for writing a previously captured HTTP response
/// back to the current <see cref="HttpContext"/>.
/// </summary>
public interface IHttpResponseWriter
{
    /// <summary>
    /// Writes the specified captured response to the current HTTP response.
    /// </summary>
    /// <param name="context">
    /// The current HTTP context whose response will be populated.
    /// </param>
    /// <param name="response">
    /// The captured response containing the body, status code, and headers
    /// to write.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous write operation.
    /// </returns>
    Task WriteAsync(
        HttpContext context,
        CapturedResponse response);
}
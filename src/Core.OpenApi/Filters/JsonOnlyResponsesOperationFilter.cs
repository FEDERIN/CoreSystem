using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Core.OpenApi.Filters;

/// <summary>
/// Removes the redundant text media types that ASP.NET Core reports next to a JSON response.
/// </summary>
/// <remarks>
/// When a response documents <c>application/json</c>, the framework also advertises
/// <c>text/plain</c> and <c>text/json</c>. Those alternatives add noise to generated clients
/// and are removed only when a JSON representation exists.
/// </remarks>
public sealed class JsonOnlyResponsesOperationFilter : IOperationFilter
{
    /// <summary>
    /// Applies the filter to one operation.
    /// </summary>
    /// <param name="operation">The operation being generated.</param>
    /// <param name="context">The filter context.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="operation"/> is <see langword="null"/>.
    /// </exception>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(operation);

        foreach (var response in operation.Responses.Values)
        {
            if (response.Content is null || response.Content.Count == 0)
            {
                continue;
            }

            if (!response.Content.ContainsKey("application/json"))
            {
                continue;
            }

            response.Content.Remove("text/plain");
            response.Content.Remove("text/json");
        }
    }
}

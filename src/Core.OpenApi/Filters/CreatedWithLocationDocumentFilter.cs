using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Core.OpenApi.Filters;

/// <summary>
/// Documents the <c>Location</c> header on every <c>201 Created</c> response.
/// </summary>
/// <remarks>
/// The header is added only when the operation does not declare it already, so the filter
/// can be applied more than once without producing duplicates.
/// </remarks>
public sealed class CreatedWithLocationDocumentFilter : IDocumentFilter
{
    /// <summary>
    /// Applies the filter to the generated document.
    /// </summary>
    /// <param name="swaggerDoc">The document being generated.</param>
    /// <param name="context">The filter context.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="swaggerDoc"/> is <see langword="null"/>.
    /// </exception>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        ArgumentNullException.ThrowIfNull(swaggerDoc);

        foreach (var path in swaggerDoc.Paths.Values)
        {
            foreach (var operation in path.Operations.Values)
            {
                if (!operation.Responses.TryGetValue("201", out var response))
                {
                    continue;
                }

                response.Headers ??= new Dictionary<string, OpenApiHeader>();

                response.Headers.TryAdd("Location", new OpenApiHeader
                {
                    Description = "URL of the newly created resource",
                    Schema = new OpenApiSchema
                    {
                        Type = "string",
                        Format = "uri"
                    }
                });
            }
        }
    }
}

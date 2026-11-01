using Microsoft.OpenApi.Models;

namespace Core.OpenApi.Abstractions;

/// <summary>
/// Produces an OpenAPI document from a given source (host, endpoints, metadata).
/// </summary>
public interface IOpenApiDocumentProvider
{
    /// <summary>
    /// Builds and returns the OpenAPI document.
    /// </summary>
    Task<OpenApiDocument> GetDocumentAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Builds an <see cref="OpenApiDocument"/> incrementally.
/// </summary>
public interface IOpenApiDocumentBuilder
{
    /// <summary>
    /// Builds and returns the resulting document.
    /// </summary>
    OpenApiDocument Build();
}
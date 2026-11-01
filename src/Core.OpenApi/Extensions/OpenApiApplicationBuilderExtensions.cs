using Core.OpenApi.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Swashbuckle.AspNetCore.Swagger;

namespace Core.OpenApi.Extensions;

/// <summary>
/// Provides extension methods that expose the patched OpenAPI document and Swagger UI.
/// </summary>
public static class OpenApiApplicationBuilderExtensions
{
    /// <summary>
    /// Maps the endpoint that serves the patched document and configures Swagger UI.
    /// </summary>
    /// <param name="app">
    /// The application to configure.
    /// </param>
    /// <returns>
    /// The same <see cref="WebApplication"/> instance for chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="app"/> is <see langword="null"/>.
    /// </exception>
    /// <remarks>
    /// Call this method after <c>AddCoreOpenApi</c> has registered the required services.
    /// The endpoint is served through GET and is not written to the document itself.
    /// </remarks>
    public static WebApplication UseCoreOpenApi(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNull(app);

        var options = app.Services.GetRequiredService<IOptions<CoreOpenApiOptions>>().Value;

        var originalRouteTemplate = options.ResolvedOriginalJsonRouteTemplate;
        if (!string.IsNullOrWhiteSpace(originalRouteTemplate))
        {
            app.UseSwagger(setup => setup.RouteTemplate = originalRouteTemplate);
        }

        var jsonRoute = options.ResolvedJsonEndpointRoute;

        var endpoint = app.MapGet(
            jsonRoute,
            async (ISwaggerProvider provider, IMemoryCache cache, HttpContext context) =>
            {
                var document = GetDocument(provider, cache, options);

                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(document).ConfigureAwait(false);
            });

        options.ConfigureJsonEndpoint?.Invoke(endpoint);

        app.UseSwaggerUI(ui =>
        {
            ui.SwaggerEndpoint(jsonRoute, $"{options.Title} {options.DocumentName}");
            ui.RoutePrefix = options.SwaggerRoutePrefix;
        });

        return app;
    }

    /// <summary>
    /// Returns the document from the cache, or builds and stores it.
    /// </summary>
    /// <param name="provider">The Swagger provider.</param>
    /// <param name="cache">The memory cache.</param>
    /// <param name="options">The CoreSystem options.</param>
    /// <returns>The serialized document.</returns>
    private static string GetDocument(
        ISwaggerProvider provider,
        IMemoryCache cache,
        CoreOpenApiOptions options)
    {
        if (!options.CachePatchedDocument)
        {
            return BuildDocument(provider, options);
        }

        var cacheKey = string.Concat("Core.OpenApi:", options.DocumentName);

        if (cache.TryGetValue(cacheKey, out string? cached) && cached is not null)
        {
            return cached;
        }

        var document = BuildDocument(provider, options);
        cache.Set(cacheKey, document);

        return document;
    }

    /// <summary>
    /// Serializes the document and applies the configured patch.
    /// </summary>
    /// <param name="provider">The Swagger provider.</param>
    /// <param name="options">The CoreSystem options.</param>
    /// <returns>The serialized document.</returns>
    private static string BuildDocument(
        ISwaggerProvider provider,
        CoreOpenApiOptions options)
    {
        var document = provider.GetSwagger(options.DocumentName);
        var json = document.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);

        return options.EnableDocumentPatch
            ? OpenApiDocumentPatcher.Patch(json, options.ProblemResponses)
            : json;
    }
}

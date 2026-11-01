using System.Reflection;
using Core.OpenApi.Filters;
using Core.OpenApi.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Core.OpenApi.DependencyInjection;

/// <summary>
/// Provides extension methods for registering CoreSystem OpenAPI services.
/// </summary>
public static class OpenApiRegistration
{
    /// <summary>
    /// Registers Swagger generation, the CoreSystem filters and the endpoint that serves the
    /// patched document.
    /// </summary>
    /// <param name="services">
    /// The service collection to configure.
    /// </param>
    /// <param name="configure">
    /// An optional callback that customizes the generated document.
    /// </param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance for chaining.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="services"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the document name, the route prefix or the version is empty.
    /// </exception>
    /// <remarks>
    /// Call this method once per service collection. Pair it with
    /// <c>UseCoreOpenApi</c> after the application is built.
    /// </remarks>
    public static IServiceCollection AddCoreOpenApi(
        this IServiceCollection services,
        Action<CoreOpenApiOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var options = new CoreOpenApiOptions();
        configure?.Invoke(options);

        options.Title = string.IsNullOrWhiteSpace(options.Title)
            ? Assembly.GetEntryAssembly()?.GetName().Name ?? "API"
            : options.Title;

        options.Validate();

        services.TryAddSingleton<IOptions<CoreOpenApiOptions>>(
            new OptionsWrapper<CoreOpenApiOptions>(options));
        services.AddMemoryCache();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(gen => ConfigureSwaggerGen(gen, options));

        return services;
    }

    /// <summary>
    /// Configures the Swagger generator with the CoreSystem conventions.
    /// </summary>
    /// <param name="gen">The generator options.</param>
    /// <param name="options">The CoreSystem options.</param>
    private static void ConfigureSwaggerGen(
        SwaggerGenOptions gen,
        CoreOpenApiOptions options)
    {
        gen.SwaggerDoc(options.DocumentName, new OpenApiInfo
        {
            Title = options.Title,
            Version = options.Version,
            Description = options.Description
        });

        if (options.Bearer.Enabled)
        {
            AddBearerSecurity(gen, options.Bearer);
        }

        foreach (var server in options.Servers)
        {
            gen.AddServer(new OpenApiServer
            {
                Url = server.Url,
                Description = server.Description
            });
        }

        gen.SupportNonNullableReferenceTypes();

        IncludeXmlComments(gen, options);

        gen.DocumentFilter<CreatedWithLocationDocumentFilter>();
        gen.OperationFilter<JsonOnlyResponsesOperationFilter>();

        options.ConfigureSwaggerGen?.Invoke(gen);
    }

    /// <summary>
    /// Registers the bearer security scheme and applies it to every operation.
    /// </summary>
    /// <param name="gen">The generator options.</param>
    /// <param name="bearer">The bearer configuration.</param>
    private static void AddBearerSecurity(
        SwaggerGenOptions gen,
        BearerSecurityOptions bearer)
    {
        gen.AddSecurityDefinition(bearer.SchemeName, new OpenApiSecurityScheme
        {
            Name = bearer.HeaderName,
            Type = SecuritySchemeType.Http,
            Scheme = bearer.Scheme,
            BearerFormat = bearer.BearerFormat,
            In = ParameterLocation.Header,
            Description = bearer.Description
        });

        gen.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = bearer.SchemeName
                    }
                },
                Array.Empty<string>()
            }
        });
    }

    /// <summary>
    /// Includes the XML documentation files that describe the API.
    /// </summary>
    /// <param name="gen">The generator options.</param>
    /// <param name="options">The CoreSystem options.</param>
    private static void IncludeXmlComments(
        SwaggerGenOptions gen,
        CoreOpenApiOptions options)
    {
        var files = options.XmlCommentFiles.Count > 0
            ? options.XmlCommentFiles
            : Directory.GetFiles(AppContext.BaseDirectory, "*.xml");

        foreach (var file in files)
        {
            if (File.Exists(file))
            {
                gen.IncludeXmlComments(file, options.IncludeControllerXmlComments);
            }
        }
    }
}

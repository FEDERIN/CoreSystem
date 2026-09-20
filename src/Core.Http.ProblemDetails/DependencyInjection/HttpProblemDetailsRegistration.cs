using Core.Http.ProblemDetails.Handlers;
using Core.Http.ProblemDetails.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Core.Http.ProblemDetails.DependencyInjection;

/// <summary>Registers reusable RFC 9457 problem-details support.</summary>
public static class HttpProblemDetailsRegistration
{
    /// <summary>Registers RFC 9457 serialization and optional CoreSystem problem customization.</summary>
    public static IServiceCollection AddCoreProblemDetails(
        this IServiceCollection services,
        Action<CoreProblemDetailsOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddProblemDetails();
        if (configure is not null)
        {
            services.Configure(configure);
        }

        return services;
    }

    /// <summary>Registers the mapper-backed exception handler.</summary>
    public static IServiceCollection AddCoreExceptionHandler<TMapper>(this IServiceCollection services)
        where TMapper : class, IExceptionProblemMapper
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddScoped<TMapper>();
        services.AddExceptionHandler<CoreExceptionHandler<TMapper>>();
        return services;
    }
}

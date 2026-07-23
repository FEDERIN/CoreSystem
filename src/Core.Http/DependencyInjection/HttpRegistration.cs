using Core.Http.Abstractions;
using Core.Http.Responses;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Http.DependencyInjection;

/// <summary>
/// Provides extension methods for registering Core.Http services.
/// </summary>
public static class HttpRegistration
{
    /// <summary>
    /// Registers the services required by Core.Http.
    /// </summary>
    /// <param name="services">
    /// The service collection to configure.
    /// </param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance for chaining.
    /// </returns>
    public static IServiceCollection AddCoreHttp(
        this IServiceCollection services)
    {
        services.AddSingleton<IResponseCapture, ResponseCapture>();
        services.AddSingleton<IHttpResponseWriter, HttpResponseWriter>();

        return services;
    }
}
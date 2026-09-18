using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Observability.Extensions;

internal static class HealthCheckExtensions
{
    public static IServiceCollection AddCoreHealthChecks(
    this IServiceCollection services,
    IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var builder = services.AddHealthChecks();

        foreach (var contributor in HealthCheckContributorRegistry.GetRegistered(services))
        {
            contributor.RegisterHealthChecks(builder, configuration);
        }

        return services;
    }
}

using Microsoft.Extensions.DependencyInjection;

namespace Core.Observability.Extensions;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddCoreHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks();
        return services;
    }
}
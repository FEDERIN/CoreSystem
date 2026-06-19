using Core.Observability.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Observability.Extensions;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddCoreHealthChecks(
    this IServiceCollection services,
    IConfiguration configuration)
    {
        var builder = services.AddHealthChecks();

        var contributors = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => typeof(IHealthCheckContributor).IsAssignableFrom(p) && !p.IsInterface);

        foreach (var type in contributors)
        {
            var contributor = (IHealthCheckContributor)Activator.CreateInstance(type)!;
            contributor.RegisterHealthChecks(builder, configuration);
        }

        return services;
    }
}
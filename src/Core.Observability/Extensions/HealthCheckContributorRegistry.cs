using Core.Observability.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Observability.Extensions;

internal static class HealthCheckContributorRegistry
{
    public static IReadOnlyCollection<IHealthCheckContributor> GetRegistered(
        IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        return [.. services
            .Where(descriptor =>
                descriptor.ServiceType == typeof(IHealthCheckContributor))
            .Select(descriptor => descriptor.ImplementationInstance)
            .OfType<IHealthCheckContributor>()
            .Distinct()];
    }
}

using Core.Observability.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Observability.Extensions;

internal static class ObservabilityContributorRegistry
{
    public static IReadOnlyCollection<IObservabilityContributor> GetRegistered(
        IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        return services
            .Where(descriptor =>
                descriptor.ServiceType == typeof(IObservabilityContributor))
            .Select(descriptor => descriptor.ImplementationInstance)
            .OfType<IObservabilityContributor>()
            .Distinct()
            .ToArray();
    }
}

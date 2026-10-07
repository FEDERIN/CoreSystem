using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Observability.Abstractions;

/// <summary>Extension point for modules that contribute observability (activity sources and service registrations).</summary>
public interface IObservabilityContributor
{
    /// <summary>Returns the activity source names provided by this contributor.</summary>
    IEnumerable<string> GetActivitySources();

    /// <summary>Registers observability services into the dependency injection container.</summary>
    void ConfigureObservability(IServiceCollection services, IConfiguration configuration);
}

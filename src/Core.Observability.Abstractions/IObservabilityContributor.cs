using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Observability.Abstractions;

public interface IObservabilityContributor
{
    IEnumerable<string> GetActivitySources();

    void ConfigureObservability(IServiceCollection services, IConfiguration configuration);
}
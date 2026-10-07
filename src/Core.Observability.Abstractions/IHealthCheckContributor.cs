using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Observability.Abstractions;

/// <summary>Extension point for modules that contribute health checks.</summary>
public interface IHealthCheckContributor
{
    /// <summary>Registers health checks into the health checks builder.</summary>
    void RegisterHealthChecks(IHealthChecksBuilder builder, IConfiguration configuration);
}

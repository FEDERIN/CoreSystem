using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Observability.Abstractions;

public interface IHealthCheckContributor
{
    void RegisterHealthChecks(IHealthChecksBuilder builder, IConfiguration configuration);
}
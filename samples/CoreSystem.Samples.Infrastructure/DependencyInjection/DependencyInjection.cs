using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoreSystem.Samples.Infrastructure.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services
            .AddRepositories()
            .AddIdempotencyInfrastructure(config)
            .AddCacheInfrastructure(config)
            .AddExceptionHandling();

        return services;
    }
}
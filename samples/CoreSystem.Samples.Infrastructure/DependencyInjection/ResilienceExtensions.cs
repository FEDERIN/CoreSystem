using Core.Resilience.DependencyInjection;
using Core.Resilience.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoreSystem.Samples.Infrastructure.DependencyInjection;

internal static class RedisResilienceExtensions
{
    public static IServiceCollection AddRedisResilienceInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var section = configuration.GetSection("Core:Resilience");

        if (!section.Exists())
        {
            throw new InvalidOperationException(
                "The 'Core:Resilience' configuration section is required.");
        }

        var options = new ResilienceOptions();
        section.Bind(options);

        services.AddCoreResilience(
            builder => builder.CopyFrom(options));

        return services;
    }
}
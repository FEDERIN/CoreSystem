using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Observability.Correlation;

/// <summary>Registers CoreSystem request correlation services and middleware.</summary>
public static class CorrelationRegistration
{
    public static IServiceCollection AddCoreCorrelation(
        this IServiceCollection services,
        Action<CorrelationOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var optionsBuilder = services.AddOptions<CorrelationOptions>();
        if (configure is not null)
        {
            optionsBuilder.Configure(configure);
        }

        return services;
    }

    public static IServiceCollection AddCoreCorrelation(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddOptions<CorrelationOptions>()
            .Bind(configuration.GetSection(CorrelationOptions.SectionName));

        return services;
    }

    public static IApplicationBuilder UseCoreCorrelationId(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);
        return app.UseMiddleware<CorrelationIdMiddleware>();
    }
}

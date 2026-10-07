using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Correlation;

/// <summary>Registers CoreSystem request correlation services and middleware.</summary>
public static class CorrelationRegistration
{
    /// <summary>Registers correlation services using a delegate-based configuration.</summary>
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

    /// <summary>Registers correlation services bound from configuration.</summary>
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

    /// <summary>Adds the correlation ID middleware to the pipeline.</summary>
    public static IApplicationBuilder UseCoreCorrelationId(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);
        return app.UseMiddleware<CorrelationIdMiddleware>();
    }
}

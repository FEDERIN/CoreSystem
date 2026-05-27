using Core.Idempotency.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Idempotency;

public static class IdempotencyRegistrationExtensions
{
    public static IServiceCollection AddIdempotencyProvider(
    this IServiceCollection services,
    IConfiguration configuration)
    {
        var section = configuration.GetSection("Idempotency");
        var options = section.Get<IdempotencyOptions>()
            ?? throw new InvalidOperationException("Sección 'Idempotency' not found.");

        if (options == null || !options.Enabled)
        {
            return services;
        }

        var connectionString = configuration.GetConnectionString("Idempotency")
            ?? throw new ArgumentNullException(nameof(configuration), "ConnectionString 'Idempotency' is required.");

        var provider = options.Provider?.Trim().ToUpperInvariant();

        if (string.IsNullOrEmpty(provider))
        {
            throw new InvalidOperationException("Idempotency provider is not specified in the configuration.");
        }

        var supportedProviders = new[] { "POSTGRESQL", "REDIS" };

        if(!supportedProviders.Contains(provider, StringComparer.OrdinalIgnoreCase))
        {
            throw new NotSupportedException($"Provider '{options.Provider}' is not supported.");
        }

        if (provider == "POSTGRESQL")
        {
            services.AddPostgresIdempotency(connectionString, opt => section.Bind(opt));
        }

        if(provider == "REDIS")
        {
            services.AddRedisIdempotency(connectionString, opt => section.Bind(opt));
        }

        return services;
    }
}
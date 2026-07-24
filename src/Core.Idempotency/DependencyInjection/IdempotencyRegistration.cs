using Core.Idempotency.Abstractions;
using Core.Idempotency.Middleware;
using Core.Idempotency.Options;
using Core.Serialization;
using Core.Serialization.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Idempotency.DependencyInjection;

public static class IdempotencyRegistration
{
    public static IServiceCollection AddCoreIdempotency(
        this IServiceCollection services,
        Action<IdempotencyOptions> configure)
    {
        var options = new IdempotencyOptions();

        configure(options);

        services
            .AddSingleton(options)
            .AddCoreSerialization(serialization =>
            {
                serialization.DefaultSerializer = SerializerType.Json;
            })
            .AddIdempotencyDiagnostics();

        if (!options.Enabled)
        {
            return services;
        }

        switch (options.Provider)
        {
            case IdempotencyProviderType.Redis:
                services.AddIdempotencyRedis(options);
                break;

            case IdempotencyProviderType.PostgreSQL:
                services.AddIdempotencyPostgreSql(options);
                break;

            default:
                throw new NotSupportedException(
                    $"Provider '{options.Provider}' is not supported.");
        }

        services.AddIdempotencyServices();

        return services;
    }

    public static IApplicationBuilder UseCoreIdempotency(
        this IApplicationBuilder app)
    {
        var options = app.ApplicationServices.GetRequiredService<IdempotencyOptions>();

        if (!options.Enabled)
        {
            return app;
        }

        return app.UseMiddleware<IdempotencyMiddleware>();
    }
}
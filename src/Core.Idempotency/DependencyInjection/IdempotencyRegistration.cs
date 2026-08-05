using Core.Http.DependencyInjection;
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
            .AddCoreHttp()
            .AddIdempotencyDiagnostics()
            .AddFingerprint();

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
                throw new NotSupportedException(IdempotencyMessages.UnsupportedProvider(options.Provider));
        }
        services.AddIdempotencyServices();

        return services;
    }

    public static IApplicationBuilder UseCoreIdempotency(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        if (app.ApplicationServices.GetService<IIdempotencyService>() is null)
        {
            throw new InvalidOperationException(IdempotencyMessages.MissingRegistration);
        }

        return app.UseMiddleware<IdempotencyMiddleware>();
    }
}
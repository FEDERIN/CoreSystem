using Core.Observability.Abstractions;
using Core.RateLimiting.Abstractions;
using Core.RateLimiting.Diagnostics;
using Core.RateLimiting.Internal;
using Core.RateLimiting.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.RateLimiting;

namespace Core.RateLimiting.DependencyInjection;

/// <summary>
/// Provides extension methods for registering and configuring
/// Core.RateLimiting services in an ASP.NET Core application.
/// </summary>
public static class RateLimitingRegistration
{
    /// <summary>
    /// Registers Core.RateLimiting services and configures the global
    /// rate limiter using the specified options.
    /// </summary>
    /// <param name="services">
    /// The <see cref="IServiceCollection"/> to which the rate-limiting services are added.
    /// </param>
    /// <param name="configure">
    /// An action used to configure <see cref="RateLimitingOptions"/>.
    /// </param>
    /// <returns>
    /// The same <see cref="IServiceCollection"/> instance so that additional
    /// service registrations can be chained.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="services"/> or <paramref name="configure"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when the configured rate-limiting options contain an invalid
    /// policy name or subject claim type.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when the configured permit limit, window, or queue limit
    /// contains an invalid value.
    /// </exception>
    public static IServiceCollection AddCoreRateLimiting(
        this IServiceCollection services,
        Action<RateLimitingOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        var options = new RateLimitingOptions();

        configure(options);
        options.Validate();

        if (!options.Enabled)
            return services;

        services.AddSingleton(options);
        services.AddProblemDetails();
        services.AddMetrics();
        services.AddSingleton<RateLimitingMetrics>();
        services.AddSingleton<IObservabilityContributor, RateLimitingObservabilityContributor>();
        services.AddSingleton<IRateLimitPartitionKeyResolver, ClientPartitionKeyResolver>();
        services.AddSingleton<RateLimitRejectionHandler>();

        services.AddRateLimiter(rateLimiterOptions =>
        {
            rateLimiterOptions.RejectionStatusCode =
                StatusCodes.Status429TooManyRequests;

            var limiterOptionsTemplate = new FixedWindowRateLimiterOptions
            {
                PermitLimit = options.PermitLimit,
                Window = options.Window,
                QueueLimit = options.QueueLimit,
                QueueProcessingOrder = options.QueueProcessingOrder,
                AutoReplenishment = options.AutoReplenishment
            };

            rateLimiterOptions.GlobalLimiter =
                PartitionedRateLimiter.Create<HttpContext, string>(context =>
                {
                    var resolver = context.RequestServices
                        .GetRequiredService<IRateLimitPartitionKeyResolver>();

                    return RateLimitPartition.GetFixedWindowLimiter(
                        resolver.Resolve(context),
                        _ => limiterOptionsTemplate);
                });

            rateLimiterOptions.OnRejected =
                (context, cancellationToken) =>
                    context.HttpContext.RequestServices
                        .GetRequiredService<RateLimitRejectionHandler>()
                        .HandleAsync(context, cancellationToken);
        });

        return services;
    }

    /// <summary>
    /// Adds the Core.RateLimiting middleware to the application's request pipeline.
    /// </summary>
    /// <param name="app">
    /// The <see cref="IApplicationBuilder"/> used to configure the application's
    /// HTTP request pipeline.
    /// </param>
    /// <returns>
    /// The same <see cref="IApplicationBuilder"/> instance so that additional
    /// middleware configuration can be chained.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="app"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when Core.RateLimiting has not been registered through
    /// <see cref="AddCoreRateLimiting(IServiceCollection, Action{RateLimitingOptions})"/>.
    /// </exception>
    public static IApplicationBuilder UseCoreRateLimiting(
        this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        var options = app.ApplicationServices.GetService<RateLimitingOptions>()
            ?? throw new InvalidOperationException(
                "Core.RateLimiting has not been registered. " +
                "Call services.AddCoreRateLimiting(...).");

        return options.Enabled
            ? app.UseRateLimiter()
            : app;
    }
}

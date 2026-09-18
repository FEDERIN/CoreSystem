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

public static class RateLimitingRegistration
{
    public static IServiceCollection AddCoreRateLimiting(this IServiceCollection services, Action<RateLimitingOptions> configure)
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
            rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            var limiterOptionsTemplate = new FixedWindowRateLimiterOptions
            {
                PermitLimit = options.PermitLimit,
                Window = options.Window,
                QueueLimit = options.QueueLimit,
                QueueProcessingOrder = options.QueueProcessingOrder,
                AutoReplenishment = options.AutoReplenishment
            };
            rateLimiterOptions.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            {
                var resolver = context.RequestServices.GetRequiredService<IRateLimitPartitionKeyResolver>();
                return RateLimitPartition.GetFixedWindowLimiter(resolver.Resolve(context), _ => limiterOptionsTemplate);
            });
            rateLimiterOptions.OnRejected = (context, cancellationToken) => context.HttpContext.RequestServices.GetRequiredService<RateLimitRejectionHandler>().HandleAsync(context, cancellationToken);
        });

        return services;
    }

    public static IApplicationBuilder UseCoreRateLimiting(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);
        var options = app.ApplicationServices.GetService<RateLimitingOptions>()
            ?? throw new InvalidOperationException("Core.RateLimiting has not been registered. Call services.AddCoreRateLimiting(...).");

        return options.Enabled ? app.UseRateLimiter() : app;
    }
}

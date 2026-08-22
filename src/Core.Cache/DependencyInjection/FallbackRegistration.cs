using Core.Cache.Pipeline.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Cache.DependencyInjection;

internal static class FallbackRegistration
{
    public static IServiceCollection AddCacheFallback(
        this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<FallbackBehavior>();

        return services;
    }
}
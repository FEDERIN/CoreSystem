using CoreSystem.Samples.Infrastructure.ExceptionHandling;
using Microsoft.Extensions.DependencyInjection;

namespace CoreSystem.Samples.Infrastructure.DependencyInjection;

internal static class ExceptionHandlingRegistration
{
    internal static IServiceCollection AddExceptionHandling(
        this IServiceCollection services)
    {
        services.AddExceptionHandler<IdempotencyExceptionHandler>();

        return services;
    }
}
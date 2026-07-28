using Core.Idempotency.ExceptionHandling;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Idempotency.DependencyInjection;

internal static class ExceptionHandlingRegistration
{
    internal static IServiceCollection AddExceptionHandling(
        this IServiceCollection services)
    {
        services.AddExceptionHandler<IdempotencyExceptionHandler>();

        return services;
    }
}
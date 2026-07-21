using Core.Resilience.Internal.Strategies;
using Microsoft.Extensions.DependencyInjection;

internal static class StrategyRegistration
{
    public static IServiceCollection AddStrategies(
        this IServiceCollection services)
    {
        services.AddSingleton<IStrategyBuilder, RetryStrategyBuilder>();
        services.AddSingleton<IStrategyBuilder, TimeoutStrategyBuilder>();
        services.AddSingleton<IStrategyBuilder, CircuitBreakerStrategyBuilder>();

        return services;
    }
}
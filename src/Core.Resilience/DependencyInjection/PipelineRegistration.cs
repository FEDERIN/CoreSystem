using Core.Resilience.Abstractions;
using Core.Resilience.Builders.Abstractions;
using Core.Resilience.Internal;
using Core.Resilience.Internal.Builders;
using Microsoft.Extensions.DependencyInjection;

internal static class PipelineRegistration
{
    public static IServiceCollection AddPipelineServices(
        this IServiceCollection services)
    {
        services.AddSingleton<IPipelineBuilder, PipelineBuilder>();

        services.AddSingleton<PipelineRegistry>();

        services.AddSingleton<IResiliencePipelineProvider, ResiliencePipelineProvider>();

        return services;
    }
}
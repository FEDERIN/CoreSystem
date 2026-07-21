using Core.Resilience.Abstractions;
using Core.Resilience.Exceptions;

namespace Core.Resilience.Internal;

internal sealed class ResiliencePipelineProvider(
    PipelineRegistry registry)
    : IResiliencePipelineProvider
{
    private readonly PipelineRegistry _registry = registry;

    public IResiliencePipeline GetPipeline(PipelineType type)
    {
        if (_registry.Pipelines.TryGetValue(type, out var pipeline))
        {
            return pipeline;
        }

        throw new ResiliencePipelineNotFoundException(type);
    }
}
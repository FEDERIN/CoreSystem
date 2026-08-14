using Core.Resilience.Abstractions;

namespace Core.Resilience.Internal;

internal sealed class NoOpResiliencePipelineProvider
    : IResiliencePipelineProvider
{
    public IResiliencePipeline GetPipeline(
        PipelineType type)
    {
        return NoOpResiliencePipeline.Instance;
    }
}
using Core.Resilience.Abstractions;
using Core.Resilience.Options;

namespace Core.Resilience.Internal;

internal sealed class PipelineRegistry(
    ResilienceOptions options,
    IPipelineBuilder builder)
{
    public IReadOnlyDictionary<PipelineType, IResiliencePipeline> Pipelines { get; } 
        = 
        options.Pipelines.ToDictionary(
            x => x.Key,
            x => builder.Build(x.Key, x.Value));
}
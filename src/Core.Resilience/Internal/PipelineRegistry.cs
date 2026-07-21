using Core.Resilience.Abstractions;
using Core.Resilience.Builders.Abstractions;
using Core.Resilience.Options;
using Microsoft.Extensions.Options;

namespace Core.Resilience.Internal;

internal sealed class PipelineRegistry(
    IOptions<ResilienceOptions> options,
    IPipelineBuilder builder)
{
    public IReadOnlyDictionary<PipelineType, IResiliencePipeline> Pipelines { get; } 
        = options.Value.Pipelines.ToDictionary(
            x => x.Key,
            x => builder.Build(x.Key, x.Value));
}
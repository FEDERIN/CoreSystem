using Core.Resilience.Options;
using Polly;

namespace Core.Resilience.Internal.Strategies;

internal interface IStrategyBuilder
{
    int Order { get; }
    void Configure(
        ResiliencePipelineBuilder builder,
        PipelineOptions options);
}
using Core.Resilience.Abstractions;
using Core.Resilience.Builders.Abstractions;
using Core.Resilience.Internal.Strategies;
using Core.Resilience.Options;

namespace Core.Resilience.Internal.Builders;

internal sealed class PipelineBuilder(IEnumerable<IStrategyBuilder> strategies) : IPipelineBuilder
{
    private readonly IReadOnlyList<IStrategyBuilder> _strategies = [.. strategies.OrderBy(x => x.Order)];

    public IResiliencePipeline Build(
        PipelineType type,
        PipelineOptions options)
    {
        var builder = new Polly.ResiliencePipelineBuilder();

        foreach (var strategy in _strategies)
        {
            strategy.Configure(builder, options);
        }

        return new ResiliencePipeline(builder.Build());
    }
}
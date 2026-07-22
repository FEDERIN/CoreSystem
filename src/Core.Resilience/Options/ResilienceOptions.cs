using Core.Resilience.Abstractions;

namespace Core.Resilience.Options;

public sealed class ResilienceOptions
{
    public Dictionary<PipelineType, PipelineOptions> Pipelines { get; } = [];

    public void AddPipeline(
        PipelineType type,
        Action<PipelineOptions> configure)
    {
        var options = new PipelineOptions();

        configure(options);

        Pipelines[type] = options;
    }

    public bool ContainsPipeline(PipelineType pipelineType)
    {
        return Pipelines.ContainsKey(pipelineType);
    }

    public void CopyFrom(ResilienceOptions source)
    {
        ArgumentNullException.ThrowIfNull(source);

        foreach (var pipeline in source.Pipelines)
        {
            Pipelines[pipeline.Key] = pipeline.Value;
        }
    }

    public PipelineOptions GetPipeline(PipelineType pipelineType)
    {
        if (!Pipelines.TryGetValue(pipelineType, out var pipeline))
        {
            throw new InvalidOperationException(
                $"Pipeline '{pipelineType}' is not configured.");
        }

        return pipeline;
    }
}
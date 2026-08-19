using Core.Resilience.Abstractions;

namespace Core.Resilience.Options;

/// <summary>
/// Represents the configuration options for resilience pipelines.
/// </summary>
public sealed class ResilienceOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether resilience pipelines are enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets the collection of configured resilience pipelines.
    /// </summary>
    public Dictionary<PipelineType, PipelineOptions> Pipelines { get; } = [];

    /// <summary>
    /// Adds a new resilience pipeline configuration.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="configure"></param>
    public void AddPipeline(
        PipelineType type,
        Action<PipelineOptions> configure)
    {
        var options = new PipelineOptions();

        configure(options);

        Pipelines[type] = options;
    }

    /// <summary>
    /// Determines whether a resilience pipeline of the specified type is configured.
    /// </summary>
    /// <param name="pipelineType"></param>
    /// <returns></returns>
    public bool ContainsPipeline(PipelineType pipelineType)
    {
        return Pipelines.ContainsKey(pipelineType);
    }

    /// <summary>
    /// Copies the configuration from another <see cref="ResilienceOptions"/> instance.
    /// </summary>
    /// <param name="source"></param>
    public void CopyFrom(ResilienceOptions source)
    {
        ArgumentNullException.ThrowIfNull(source);

        foreach (var pipeline in source.Pipelines)
        {
            Pipelines[pipeline.Key] = pipeline.Value;
        }
    }

    /// <summary>
    /// Gets the configuration for a specific resilience pipeline type.
    /// </summary>
    /// <param name="pipelineType"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
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
namespace Core.Resilience.Abstractions;

/// <summary>
/// Provides access to registered resilience pipelines.
/// </summary>
/// <remarks>
/// This abstraction allows components to resolve a resilience pipeline
/// by its <see cref="PipelineType"/> without depending on the underlying
/// resilience implementation. If a requested pipeline has not been
/// registered, an exception is thrown.
/// </remarks>
public interface IResiliencePipelineProvider
{
    /// <summary>
    /// Gets the resilience pipeline associated with the specified pipeline type.
    /// </summary>
    /// <param name="pipelineType">
    /// The type of pipeline to retrieve.
    /// </param>
    /// <returns>
    /// The registered <see cref="IResiliencePipeline"/> instance.
    /// </returns>
    /// <exception cref="Core.Resilience.Exceptions.ResiliencePipelineNotFoundException">
    /// Thrown when no resilience pipeline is registered for the specified
    /// <paramref name="pipelineType"/>.
    /// </exception>
    IResiliencePipeline GetPipeline(
        PipelineType pipelineType);
}
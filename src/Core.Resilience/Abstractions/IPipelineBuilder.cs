namespace Core.Resilience.Builders.Abstractions;

using Core.Resilience.Abstractions;
using Core.Resilience.Options;

/// <summary>
/// Defines the contract for creating resilience pipelines from a configured
/// <see cref="PipelineOptions"/> instance.
/// </summary>
/// <remarks>
/// Implementations are responsible for translating configuration options into
/// a concrete <see cref="IResiliencePipeline"/> that can execute operations
/// with the configured resilience strategies.
/// </remarks>
public interface IPipelineBuilder
{
    /// <summary>
    /// Builds a resilience pipeline for the specified pipeline type.
    /// </summary>
    /// <param name="type">
    /// The logical type of pipeline to create.
    /// </param>
    /// <param name="options">
    /// The configuration options that define the resilience strategies to apply.
    /// </param>
    /// <returns>
    /// A configured <see cref="IResiliencePipeline"/> instance.
    /// </returns>
    IResiliencePipeline Build(
        PipelineType type,
        PipelineOptions options);
}
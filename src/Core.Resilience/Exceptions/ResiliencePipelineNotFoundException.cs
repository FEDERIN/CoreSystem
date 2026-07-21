using Core.Resilience.Abstractions;

namespace Core.Resilience.Exceptions;

/// <summary>
/// Represents an exception that is thrown when a requested resilience pipeline
/// has not been registered in the current application.
/// </summary>
/// <remarks>
/// This exception is typically thrown by <see cref="IResiliencePipelineProvider"/>
/// when attempting to resolve a pipeline that has not been configured.
/// </remarks>
/// <param name="pipelineType">
/// The type of the resilience pipeline that could not be found.
/// </param>
public sealed class ResiliencePipelineNotFoundException(PipelineType pipelineType)
    : Exception($"The resilience pipeline '{pipelineType}' is not registered.")
{
}
namespace Core.Resilience.Abstractions;

/// <summary>
/// Represents a configured resilience pipeline that executes operations
/// through one or more resilience strategies such as retries, timeouts,
/// and circuit breakers.
/// </summary>
/// <remarks>
/// A resilience pipeline encapsulates the execution policies associated
/// with a specific <see cref="PipelineType"/>.
///
/// Consumers execute operations through this abstraction without depending
/// directly on Polly, allowing resilience strategies to evolve without
/// affecting application code.
///
/// Instances are created by the Core.Resilience infrastructure and are
/// typically resolved through <see cref="IResiliencePipelineProvider"/>.
/// </remarks>
public interface IResiliencePipeline
{
    /// <summary>
    /// Executes the specified asynchronous operation through the configured
    /// resilience pipeline.
    /// </summary>
    /// <param name="operation">
    /// The asynchronous operation to execute. The provided
    /// <see cref="CancellationToken"/> should be observed by the operation.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the execution.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous execution of the operation.
    /// </returns>
    Task ExecuteAsync(
        Func<CancellationToken, Task> operation,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes the specified asynchronous operation through the configured
    /// resilience pipeline and returns its result.
    /// </summary>
    /// <typeparam name="TResult">
    /// The type of the value returned by the operation.
    /// </typeparam>
    /// <param name="operation">
    /// The asynchronous operation to execute. The provided
    /// <see cref="CancellationToken"/> should be observed by the operation.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel the execution.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous execution of the operation
    /// and contains its result.
    /// </returns>
    Task<TResult> ExecuteAsync<TResult>(
        Func<CancellationToken, Task<TResult>> operation,
        CancellationToken cancellationToken = default);
}
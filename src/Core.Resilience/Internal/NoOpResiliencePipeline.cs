using Core.Resilience.Abstractions;

namespace Core.Resilience.Internal;

/// <summary>A no-op resilience pipeline that executes operations without any policy.</summary>
public sealed class NoOpResiliencePipeline : IResiliencePipeline
{
    /// <summary>Shared instance since the pipeline is stateless.</summary>
    public static readonly NoOpResiliencePipeline Instance = new();

    private NoOpResiliencePipeline()
    {
    }

    /// <summary>Executes the operation without any resilience policy.</summary>
    public Task ExecuteAsync(
        Func<CancellationToken, Task> operation,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(operation);

        return operation(cancellationToken);
    }

    /// <summary>Executes the operation without any resilience policy.</summary>
    public Task<TResult> ExecuteAsync<TResult>(
        Func<CancellationToken, Task<TResult>> operation,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(operation);

        return operation(cancellationToken);
    }
}

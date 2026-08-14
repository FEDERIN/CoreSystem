using Core.Resilience.Abstractions;

namespace Core.Resilience.Internal;

public sealed class NoOpResiliencePipeline : IResiliencePipeline
{
    public static readonly NoOpResiliencePipeline Instance = new();

    private NoOpResiliencePipeline()
    {
    }

    public Task ExecuteAsync(
        Func<CancellationToken, Task> operation,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(operation);

        return operation(ct);
    }

    public Task<TResult> ExecuteAsync<TResult>(
        Func<CancellationToken, Task<TResult>> operation,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(operation);

        return operation(ct);
    }
}
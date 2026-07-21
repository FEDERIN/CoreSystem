using Core.Resilience.Abstractions;

namespace Core.Resilience.Internal;

internal sealed class ResiliencePipeline(
    Polly.ResiliencePipeline pipeline)
    : IResiliencePipeline
{
    private readonly Polly.ResiliencePipeline _pipeline = pipeline;

    public async Task ExecuteAsync(
        Func<CancellationToken, Task> operation,
        CancellationToken cancellationToken = default)
    {
        await _pipeline.ExecuteAsync(
            async token =>
            {
                await operation(token);
            },
            cancellationToken);
    }

    public async Task<TResult> ExecuteAsync<TResult>(
        Func<CancellationToken, Task<TResult>> operation,
        CancellationToken cancellationToken = default)
    {
        return await _pipeline.ExecuteAsync(
            async token =>
            {
                return await operation(token);
            },
            cancellationToken);
    }
}
using Polly;


namespace Core.Resilience.UnitTests.Internal;

public sealed class ResiliencePipelineTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldExecuteOperation()
    {
        // Arrange
        var pollyPipeline = new ResiliencePipelineBuilder().Build();

        var pipeline = new Resilience.Internal.ResiliencePipeline(
            pollyPipeline);

        var executed = false;

        // Act
        await pipeline.ExecuteAsync(_ =>
        {
            executed = true;
            return Task.CompletedTask;
        }, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(executed);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnResult()
    {
        // Arrange
        var pollyPipeline = new ResiliencePipelineBuilder().Build();

        var pipeline = new Resilience.Internal.ResiliencePipeline(
            pollyPipeline);

        // Act
        var result = await pipeline.ExecuteAsync(_ =>
            Task.FromResult(42), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(42, result);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldPropagateException()
    {
        // Arrange
        var pollyPipeline = new ResiliencePipelineBuilder().Build();

        var pipeline = new Resilience.Internal.ResiliencePipeline(
            pollyPipeline);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            pipeline.ExecuteAsync(_ =>
                throw new InvalidOperationException(), TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task ExecuteAsync_ShouldPassCancellationToken()
    {
        // Arrange
        var pollyPipeline = new ResiliencePipelineBuilder().Build();

        var pipeline = new Resilience.Internal.ResiliencePipeline(
            pollyPipeline);

        using var cts = new CancellationTokenSource();

        CancellationToken received = default;

        // Act
        await pipeline.ExecuteAsync(token =>
        {
            received = token;
            return Task.CompletedTask;
        }, cts.Token);

        // Assert
        Assert.Equal(cts.Token, received);
    }


}
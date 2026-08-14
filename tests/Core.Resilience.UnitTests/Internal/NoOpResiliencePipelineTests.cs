using Core.Resilience.Internal;

namespace Core.Resilience.UnitTests.Internal;

public sealed class NoOpResiliencePipelineTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldExecuteOperation()
    {
        // Arrange
        var pipeline = NoOpResiliencePipeline.Instance;

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
    public async Task ExecuteAsync_ShouldReturnOperationResult()
    {
        // Arrange
        var pipeline = NoOpResiliencePipeline.Instance;

        // Act
        var result = await pipeline.ExecuteAsync(_ => Task.FromResult(42), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(42, result);
    }
}
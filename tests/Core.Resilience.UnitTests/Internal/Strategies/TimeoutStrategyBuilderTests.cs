using Core.Resilience.Diagnostics;
using Core.Resilience.Internal.Constants;
using Core.Resilience.Options;
using Polly;
using Polly.Timeout;

namespace Core.Resilience.UnitTests.Internal.Strategies;

public sealed class TimeoutStrategyBuilderTests
{
    private readonly TimeoutStrategyBuilder _builder;

    public TimeoutStrategyBuilderTests()
    {
        var metrics = new ResilienceMetrics(new TestMeterFactory());

        _builder = new TimeoutStrategyBuilder(metrics);
    }

    [Fact]
    public void Order_ShouldReturnTimeoutOrder()
    {
        // Assert
        Assert.Equal(
            StrategyOrder.Timeout,
            _builder.Order);
    }

    [Fact]
    public void Configure_ShouldNotAddTimeout_WhenTimeoutIsNull()
    {
        // Arrange
        var builder = new ResiliencePipelineBuilder();

        var options = new PipelineOptions();

        // Act
        _builder.Configure(builder, options);

        // Assert
        Assert.NotNull(builder.Build());
    }


    [Fact]
    public void Timeout_ShouldThrow_WhenValueIsNegative()
    {
        var options = new TimeoutOptions();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            options.Timeout = TimeSpan.FromSeconds(-1));
    }

    [Fact]
    public void Configure_ShouldConfigureTimeout_WhenTimeoutIsValid()
    {
        // Arrange
        var builder = new ResiliencePipelineBuilder();

        var options = new PipelineOptions
        {
            Timeout = new TimeoutOptions
            {
                Timeout = TimeSpan.FromMilliseconds(100)
            }
        };

        // Act
        _builder.Configure(builder, options);

        // Assert
        Assert.NotNull(builder.Build());
    }

    [Fact]
    public async Task Configure_ShouldThrowTimeoutRejectedException_WhenExecutionExceedsTimeout()
    {
        // Arrange
        var builder = new ResiliencePipelineBuilder();

        var options = new PipelineOptions
        {
            Timeout = new TimeoutOptions
            {
                Timeout = TimeSpan.FromMilliseconds(50)
            }
        };

        _builder.Configure(builder, options);

        var pipeline = builder.Build();

        // Act & Assert
        await Assert.ThrowsAsync<TimeoutRejectedException>(() =>
            pipeline.ExecuteAsync(async token =>
            {
                await Task.Delay(500, token);
            }, TestContext.Current.CancellationToken).AsTask());
    }

    [Fact]
    public async Task Configure_ShouldNotThrow_WhenExecutionCompletesBeforeTimeout()
    {
        // Arrange
        var builder = new ResiliencePipelineBuilder();

        var options = new PipelineOptions
        {
            Timeout = new TimeoutOptions
            {
                Timeout = TimeSpan.FromSeconds(1)
            }
        };

        _builder.Configure(builder, options);

        var pipeline = builder.Build();

        // Act
        await pipeline.ExecuteAsync(async token =>
        {
            await Task.Delay(10, token);
        }, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(true);
    }
}
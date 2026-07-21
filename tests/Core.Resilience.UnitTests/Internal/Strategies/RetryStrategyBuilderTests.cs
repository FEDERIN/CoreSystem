using Core.Resilience.Diagnostics;
using Core.Resilience.Internal.Constants;
using Core.Resilience.Options;
using Polly;

namespace Core.Resilience.UnitTests.Internal.Strategies;

public sealed class RetryStrategyBuilderTests
{
    private readonly RetryStrategyBuilder _builder;

    public RetryStrategyBuilderTests()
    {
        var metrics = new ResilienceMetrics(new TestMeterFactory());

        _builder = new RetryStrategyBuilder(metrics);
    }

    [Fact]
    public void Order_ShouldReturnRetryOrder()
    {
        // Assert
        Assert.Equal(
            StrategyOrder.Retry,
            _builder.Order);
    }

    [Fact]
    public void Configure_ShouldNotAddRetry_WhenRetryIsNull()
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
    public void Configure_ShouldNotAddRetry_WhenRetryIsDisabled()
    {
        // Arrange
        var builder = new ResiliencePipelineBuilder();

        var options = new PipelineOptions
        {
            Retry = new RetryOptions
            {
                Enabled = false
            }
        };

        // Act
        _builder.Configure(builder, options);

        // Assert
        Assert.NotNull(builder.Build());
    }

    [Fact]
    public void Configure_ShouldConfigureRetry_WhenRetryIsEnabled()
    {
        // Arrange
        var builder = new ResiliencePipelineBuilder();

        var options = new PipelineOptions
        {
            Retry = new RetryOptions
            {
                Enabled = true,
                MaxRetryAttempts = 2,
                Delay = TimeSpan.Zero
            }
            .Handle<TimeoutException>()
        };

        // Act
        _builder.Configure(builder, options);

        // Assert
        Assert.NotNull(builder.Build());
    }

    [Fact]
    public async Task Configure_ShouldRetryHandledExceptions()
    {
        // Arrange
        var builder = new ResiliencePipelineBuilder();

        var options = new PipelineOptions
        {
            Retry = new RetryOptions
            {
                Enabled = true,
                MaxRetryAttempts = 2,
                Delay = TimeSpan.Zero
            }
            .Handle<TimeoutException>()
        };

        _builder.Configure(builder, options);

        var pipeline = builder.Build();

        var attempts = 0;

        // Act
        await Assert.ThrowsAsync<TimeoutException>(() =>
            pipeline.ExecuteAsync(_ =>
            {
                attempts++;
                throw new TimeoutException();
            }, TestContext.Current.CancellationToken).AsTask());

        // Assert
        Assert.Equal(3, attempts);
    }

    [Fact]
    public async Task Configure_ShouldNotRetryUnhandledExceptions()
    {
        // Arrange
        var builder = new ResiliencePipelineBuilder();

        var options = new PipelineOptions
        {
            Retry = new RetryOptions
            {
                Enabled = true,
                MaxRetryAttempts = 5,
                Delay = TimeSpan.Zero
            }
            .Handle<TimeoutException>()
        };

        _builder.Configure(builder, options);

        var pipeline = builder.Build();

        var attempts = 0;

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            pipeline.ExecuteAsync(_ =>
            {
                attempts++;
                throw new InvalidOperationException();
            }, TestContext.Current.CancellationToken).AsTask());

        // Assert
        Assert.Equal(1, attempts);
    }

    [Fact]
    public async Task Configure_ShouldUseConfiguredRetryAttempts()
    {
        // Arrange
        var builder = new ResiliencePipelineBuilder();

        var options = new PipelineOptions
        {
            Retry = new RetryOptions
            {
                Enabled = true,
                MaxRetryAttempts = 5,
                Delay = TimeSpan.Zero
            }
            .Handle<TimeoutException>()
        };

        _builder.Configure(builder, options);

        var pipeline = builder.Build();

        var attempts = 0;

        // Act
        await Assert.ThrowsAsync<TimeoutException>(() =>
            pipeline.ExecuteAsync(_ =>
            {
                attempts++;
                throw new TimeoutException();
            }, TestContext.Current.CancellationToken).AsTask());

        // Assert
        Assert.Equal(6, attempts);
    }
}
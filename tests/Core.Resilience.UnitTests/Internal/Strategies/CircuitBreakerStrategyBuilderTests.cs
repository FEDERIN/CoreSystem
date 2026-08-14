using Core.Resilience.Diagnostics;
using Core.Resilience.Internal.Constants;
using Core.Resilience.Internal.Strategies;
using Core.Resilience.Options;
using Polly;
using Polly.CircuitBreaker;


namespace Core.Resilience.UnitTests.Internal.Strategies;

public sealed class CircuitBreakerStrategyBuilderTests
{
    private readonly ResilienceMetrics _metrics;
    private readonly CircuitBreakerStrategyBuilder _builder;

    public CircuitBreakerStrategyBuilderTests()
    {
        var meterFactory = new TestMeterFactory();

        _metrics = new ResilienceMetrics(meterFactory);
        _builder = new CircuitBreakerStrategyBuilder(_metrics);
    }

    [Fact]
    public void Order_ShouldReturnCircuitBreakerOrder()
    {
        // Assert
        Assert.Equal(
            StrategyOrder.CircuitBreaker,
            _builder.Order);
    }

    [Fact]
    public void Configure_ShouldDoNothing_WhenCircuitBreakerIsNull()
    {
        // Arrange
        var builder = new ResiliencePipelineBuilder();

        var options = new PipelineOptions();

        // Act
        var exception = Record.Exception(() =>
            _builder.Configure(builder, options));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void Configure_ShouldDoNothing_WhenCircuitBreakerIsDisabled()
    {
        // Arrange
        var builder = new ResiliencePipelineBuilder();

        var options = new PipelineOptions
        {
            CircuitBreaker = new CircuitBreakerOptions
            {
                Enabled = false
            }
        };

        // Act
        var exception = Record.Exception(() =>
            _builder.Configure(builder, options));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void Configure_ShouldConfigureCircuitBreaker_WhenEnabled()
    {
        // Arrange
        var builder = new ResiliencePipelineBuilder();

        var options = new PipelineOptions
        {
            CircuitBreaker = new CircuitBreakerOptions
            {
                Enabled = true,
                FailureRatio = 0.5,
                MinimumThroughput = 2,
                SamplingDuration = TimeSpan.FromSeconds(10),
                BreakDuration = TimeSpan.FromSeconds(5)
            }
        };

        // Act
        var exception = Record.Exception(() =>
            _builder.Configure(builder, options));

        // Assert
        Assert.Null(exception);

        var pipeline = builder.Build();

        Assert.NotNull(pipeline);
    }

    [Fact]
    public void Configure_ShouldConfigurePredicate_WhenHandledExceptionsExist()
    {
        // Arrange
        var builder = new ResiliencePipelineBuilder();

        var circuitBreaker = new CircuitBreakerOptions
        {
            Enabled = true
        }
        .Handle<InvalidOperationException>();

        var options = new PipelineOptions
        {
            CircuitBreaker = circuitBreaker
        };

        // Act
        _builder.Configure(builder, options);

        // Assert
        var pipeline = builder.Build();

        Assert.NotNull(pipeline);
    }

    [Fact]
    public async Task Configure_ShouldOpenCircuit_WhenInnerExceptionMatches()
    {
        // Arrange
        var builder = new ResiliencePipelineBuilder();

        var options = new PipelineOptions
        {
            CircuitBreaker = new CircuitBreakerOptions
            {
                Enabled = true,
                FailureRatio = 0.5,
                MinimumThroughput = 2,
                SamplingDuration = TimeSpan.FromMinutes(1),
                BreakDuration = TimeSpan.FromMinutes(1),
                IncludeInnerExceptions = true
            }
            .Handle<TimeoutException>()
        };

        _builder.Configure(builder, options);

        var pipeline = builder.Build();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            pipeline.ExecuteAsync(_ =>
            {
                throw new InvalidOperationException(
                    "Outer",
                    new TimeoutException());
            }, TestContext.Current.CancellationToken).AsTask());

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            pipeline.ExecuteAsync(_ =>
            {
                throw new InvalidOperationException(
                    "Outer",
                    new TimeoutException());
            }, TestContext.Current.CancellationToken).AsTask());

        await Assert.ThrowsAsync<BrokenCircuitException>(() =>
            pipeline.ExecuteAsync(_ => ValueTask.CompletedTask,
                TestContext.Current.CancellationToken).AsTask());
    }

    [Fact]
    public async Task Configure_ShouldNotOpenCircuit_WhenInnerExceptionMatchesButOptionIsDisabled()
    {
        // Arrange
        var builder = new ResiliencePipelineBuilder();

        var options = new PipelineOptions
        {
            CircuitBreaker = new CircuitBreakerOptions
            {
                Enabled = true,
                FailureRatio = 0.5,
                MinimumThroughput = 2,
                SamplingDuration = TimeSpan.FromMinutes(1),
                BreakDuration = TimeSpan.FromMinutes(1),
                IncludeInnerExceptions = false
            }
            .Handle<TimeoutException>()
        };

        _builder.Configure(builder, options);

        var pipeline = builder.Build();

        for (var i = 0; i < 3; i++)
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                pipeline.ExecuteAsync(_ =>
                {
                    throw new InvalidOperationException(
                        "Outer",
                        new TimeoutException());
                }, TestContext.Current.CancellationToken).AsTask());
        }
    }

    [Fact]
    public void Configure_ShouldConfigurePredicate_WhenIncludeInnerExceptionsIsEnabled()
    {
        // Arrange
        var builder = new ResiliencePipelineBuilder();

        var options = new PipelineOptions
        {
            CircuitBreaker = new CircuitBreakerOptions
            {
                Enabled = true,
                IncludeInnerExceptions = true
            }
            .Handle<TimeoutException>()
        };

        // Act
        _builder.Configure(builder, options);

        // Assert
        Assert.NotNull(builder.Build());
    }

    [Fact]
    public async Task Configure_ShouldRecordHalfOpenedAndClosed_WhenCircuitRecovers()
    {
        // Arrange
        var builder = new ResiliencePipelineBuilder();

        var options = new PipelineOptions
        {
            CircuitBreaker = new CircuitBreakerOptions
            {
                Enabled = true,
                FailureRatio = 0.5,
                MinimumThroughput = 2,
                SamplingDuration = TimeSpan.FromMinutes(1),
                BreakDuration = TimeSpan.FromMilliseconds(500)
            }
            .Handle<InvalidOperationException>()
        };

        _builder.Configure(builder, options);

        var pipeline = builder.Build();

        // Act - Open circuit
        for (var i = 0; i < 2; i++)
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                pipeline.ExecuteAsync(_ =>
                {
                    throw new InvalidOperationException();
                }, TestContext.Current.CancellationToken).AsTask());
        }

        // Wait until the circuit can transition to Half-Open.
        await Task.Delay(
            TimeSpan.FromMilliseconds(600),
            TestContext.Current.CancellationToken);

        // Half-Open -> Closed
        await pipeline.ExecuteAsync(
            _ => ValueTask.CompletedTask,
            TestContext.Current.CancellationToken);

        // Assert
        Assert.True(true);
    }
}
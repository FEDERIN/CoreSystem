using Core.Resilience.Diagnostics;
using Core.Resilience.Internal.Constants;
using Core.Resilience.Internal.Strategies;
using Core.Resilience.Options;
using Polly;


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
}
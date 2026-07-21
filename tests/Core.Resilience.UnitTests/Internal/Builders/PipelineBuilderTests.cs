using Core.Resilience.Abstractions;
using Core.Resilience.Internal.Builders;
using Core.Resilience.Internal.Strategies;
using Core.Resilience.Options;
using FluentAssertions;
using Moq;
using Polly;

namespace Core.Resilience.UnitTests.Internal.Builders;

public sealed class PipelineBuilderTests
{
    [Fact]
    public void Build_ShouldReturnResiliencePipeline()
    {
        // Arrange
        var sut = new PipelineBuilder([]);

        // Act
        var pipeline = sut.Build(
            PipelineType.Redis,
            new PipelineOptions());

        // Assert
        pipeline.Should().NotBeNull();
        pipeline.Should().BeAssignableTo<IResiliencePipeline>();
    }

    [Fact]
    public void Build_ShouldConfigureAllStrategies()
    {
        // Arrange
        var options = new PipelineOptions();

        var strategy1 = new Mock<IStrategyBuilder>();

        strategy1
            .Setup(x => x.Configure(
                It.IsAny<ResiliencePipelineBuilder>(),
                options));

        var strategy2 = new Mock<IStrategyBuilder>();

        strategy2
            .Setup(x => x.Configure(
                It.IsAny<ResiliencePipelineBuilder>(),
                options));

        var sut = new PipelineBuilder(
        [
            strategy1.Object,
            strategy2.Object
        ]);

        // Act
        sut.Build(
            PipelineType.Redis,
            options);

        // Assert
        strategy1.Verify(x =>
            x.Configure(
                It.IsAny<ResiliencePipelineBuilder>(),
                options),
            Times.Once);

        strategy2.Verify(x =>
            x.Configure(
                It.IsAny<ResiliencePipelineBuilder>(),
                options),
            Times.Once);
    }

    [Fact]
    public void Build_ShouldConfigureStrategiesInOrder()
    {
        // Arrange
        var executionOrder = new List<string>();

        var first = new Mock<IStrategyBuilder>();

        first.SetupGet(x => x.Order)
            .Returns(2);

        first
            .Setup(x => x.Configure(
                It.IsAny<ResiliencePipelineBuilder>(),
                It.IsAny<PipelineOptions>()))
            .Callback(() => executionOrder.Add("Retry"));

        var second = new Mock<IStrategyBuilder>();

        second.SetupGet(x => x.Order)
            .Returns(1);

        second
            .Setup(x => x.Configure(
                It.IsAny<ResiliencePipelineBuilder>(),
                It.IsAny<PipelineOptions>()))
            .Callback(() => executionOrder.Add("Timeout"));

        var third = new Mock<IStrategyBuilder>();

        third.SetupGet(x => x.Order)
            .Returns(3);

        third
            .Setup(x => x.Configure(
                It.IsAny<ResiliencePipelineBuilder>(),
                It.IsAny<PipelineOptions>()))
            .Callback(() => executionOrder.Add("Circuit"));

        var sut = new PipelineBuilder(
        [
            first.Object,
            second.Object,
            third.Object
        ]);

        // Act
        sut.Build(
            PipelineType.Redis,
            new PipelineOptions());

        // Assert
        executionOrder.Should().Equal(
            "Timeout",
            "Retry",
            "Circuit");
    }

    [Fact]
    public void Build_ShouldCreatePipeline_WhenNoStrategiesAreRegistered()
    {
        // Arrange
        var sut = new PipelineBuilder([]);

        // Act
        var pipeline = sut.Build(
            PipelineType.Redis,
            new PipelineOptions());

        // Assert
        pipeline.Should().NotBeNull();
    }
}
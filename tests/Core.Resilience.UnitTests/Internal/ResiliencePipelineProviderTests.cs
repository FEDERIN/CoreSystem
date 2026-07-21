using Core.Resilience.Abstractions;
using Core.Resilience.Builders.Abstractions;
using Core.Resilience.Exceptions;
using Core.Resilience.Internal;
using Core.Resilience.Options;
using Moq;

namespace Core.Resilience.UnitTests.Internal;

public sealed class ResiliencePipelineProviderTests
{
    [Fact]
    public void GetPipeline_ShouldReturnPipeline_WhenPipelineExists()
    {
        // Arrange
        var pipeline = new Mock<IResiliencePipeline>();

        var options = new ResilienceOptions();

        options.AddPipeline(PipelineType.Redis, _ => { });

        var builder = new Mock<IPipelineBuilder>();

        builder
            .Setup(x => x.Build(
                PipelineType.Redis,
                It.IsAny<PipelineOptions>()))
            .Returns(pipeline.Object);

        var registry = new PipelineRegistry(
            Microsoft.Extensions.Options.Options.Create(options),
            builder.Object);

        var provider = new ResiliencePipelineProvider(registry);

        // Act
        var result = provider.GetPipeline(PipelineType.Redis);

        // Assert
        Assert.Same(pipeline.Object, result);
    }

    [Fact]
    public void GetPipeline_ShouldThrow_WhenPipelineDoesNotExist()
    {
        // Arrange
        var options = new ResilienceOptions();

        var builder = new Mock<IPipelineBuilder>();

        var registry = new PipelineRegistry(
            Microsoft.Extensions.Options.Options.Create(options),
            builder.Object);

        var provider = new ResiliencePipelineProvider(registry);

        // Act & Assert
        Assert.Throws<ResiliencePipelineNotFoundException>(() =>
            provider.GetPipeline(PipelineType.Redis));
    }
}
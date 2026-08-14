using Core.Resilience.Abstractions;
using Core.Resilience.Options;
using FluentAssertions;

namespace Core.Resilience.UnitTests.Options;

public sealed class RetryOptionsTests
{
    [Fact]
    public void Handle_ShouldAddHandledExceptions()
    {
        // Arrange
        var options = new RetryOptions();

        // Act
        var result = options.Handle(
            typeof(TimeoutException),
            typeof(InvalidOperationException));

        // Assert
        result.Should().BeSameAs(options);

        options.HandledExceptions.Should().Contain(
            typeof(TimeoutException));

        options.HandledExceptions.Should().Contain(
            typeof(InvalidOperationException));
    }

    [Fact]
    public void GetPipeline_ShouldReturnConfiguredPipeline()
    {
        // Arrange
        var options = new ResilienceOptions();

        options.AddPipeline(
            PipelineType.Redis,
            pipeline =>
            {
                pipeline.Timeout = new TimeoutOptions
                {
                    Timeout = TimeSpan.FromSeconds(5)
                };
            });

        // Act
        var result = options.GetPipeline(PipelineType.Redis);

        // Assert
        result.Should().NotBeNull();
        result.Timeout.Should().NotBeNull();
        result.Timeout!.Timeout.Should()
            .Be(TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void GetPipeline_ShouldThrow_WhenPipelineIsNotConfigured()
    {
        // Arrange
        var options = new ResilienceOptions();

        // Act
        var act = () => options.GetPipeline(PipelineType.Redis);

        // Assert
        act.Should()
            .Throw<InvalidOperationException>()
            .WithMessage(
                "Pipeline 'Redis' is not configured.");
    }
}
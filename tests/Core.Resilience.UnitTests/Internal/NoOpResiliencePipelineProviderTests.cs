using Core.Resilience.Abstractions;
using Core.Resilience.Internal;
using FluentAssertions;

namespace Core.Resilience.UnitTests.Internal;

public sealed class NoOpResiliencePipelineProviderTests
{
    [Fact]
    public void GetPipeline_ShouldReturnNoOpPipeline()
    {
        // Arrange
        var provider = new NoOpResiliencePipelineProvider();

        // Act
        var result = provider.GetPipeline(PipelineType.Redis);

        // Assert
        result.Should().BeSameAs(
            NoOpResiliencePipeline.Instance);
    }
}
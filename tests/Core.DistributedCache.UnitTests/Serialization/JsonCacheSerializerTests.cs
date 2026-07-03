using Core.DistributedCache.Serialization;
using FluentAssertions;

namespace Core.DistributedCache.UnitTests.Serialization;

public sealed partial class JsonCacheSerializerTests
{
    [Fact]
    public void Deserialize_WhenBytesAreEmpty_ShouldReturnDefault()
    {
        // Arrange
        var serializer = new JsonCacheSerializer();

        var bytes = Array.Empty<byte>();

        // Act
        var result = serializer.Deserialize<string>(bytes);

        // Assert
        result.Should().BeNull();
    }
}
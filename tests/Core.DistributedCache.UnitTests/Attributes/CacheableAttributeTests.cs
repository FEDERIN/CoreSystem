using Core.DistributedCache.Attributes;
using FluentAssertions;

namespace Core.DistributedCache.UnitTests.Attributes;

public sealed class CacheableAttributeTests
{
    [Fact]
    public void Constructor_WhenNoParameters_ShouldUseDefaults()
    {
        // Act
        var attribute = new CacheableAttribute();

        // Assert
        attribute.Tag.Should().BeNull();
        attribute.ExpirationSeconds.Should().BeNull();
    }

    [Fact]
    public void Constructor_WhenExpirationIsProvided_ShouldSetExpiration()
    {
        // Act
        var attribute = new CacheableAttribute(60);

        // Assert
        attribute.ExpirationSeconds.Should().Be(60);
        attribute.Tag.Should().BeNull();
    }

    [Fact]
    public void Constructor_WhenTagIsProvided_ShouldSetTag()
    {
        // Act
        var attribute = new CacheableAttribute(0, "customers");

        // Assert
        attribute.Tag.Should().Be("customers");
        attribute.ExpirationSeconds.Should().BeNull();
    }

    [Fact]
    public void Constructor_WhenExpirationAndTagAreProvided_ShouldSetBothProperties()
    {
        // Act
        var attribute = new CacheableAttribute(300, "customers");

        // Assert
        attribute.ExpirationSeconds.Should().Be(300);
        attribute.Tag.Should().Be("customers");
    }
}
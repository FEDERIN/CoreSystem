using Core.Cache.Http.Caching;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace Core.Cache.UnitTests.Http;

public sealed class HttpCacheKeyGeneratorTests
{
    [Fact]
    public void Generate_ShouldIncludeInstanceName()
    {
        // Arrange
        var generator = new HttpCacheKeyGenerator();

        var context = new DefaultHttpContext();
        context.Request.Path = "/customers";

        // Act
        var key = generator.Generate(context);

        // Assert
        key.Should().Be("/customers:");
    }
}
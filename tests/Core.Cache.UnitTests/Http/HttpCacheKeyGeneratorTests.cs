using Core.Cache.Http.Caching;
using Core.Cache.Options;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace Core.Cache.UnitTests.Http;

public sealed class HttpCacheKeyGeneratorTests
{
    [Fact]
    public void Generate_ShouldIncludeInstanceName()
    {
        // Arrange
        var options = new CacheOptions
        {
            InstanceName = "CoreSystem"
        };

        var generator = new HttpCacheKeyGenerator(options);

        var context = new DefaultHttpContext();
        context.Request.Path = "/customers";

        // Act
        var key = generator.Generate(context);

        // Assert
        key.Should().Be("CoreSystem:/customers:");
    }
}
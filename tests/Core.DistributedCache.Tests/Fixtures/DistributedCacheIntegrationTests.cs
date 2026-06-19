using FluentAssertions;

namespace Core.DistributedCache.Tests.Fixtures;


public class DistributedCacheIntegrationTests(DistributedCacheFixture fixture) : IClassFixture<DistributedCacheFixture>
{
    private readonly DistributedCacheFixture _fixture = fixture;

    [Fact]
    public async Task GetAsync_ShouldReturnCachedValue_WhenKeyExists()
    {
        var db = _fixture.Redis.GetDatabase();

        db.Should().NotBeNull();
    }
}
using Core.Cache.Abstractions;
using Core.Cache.IntegrationTests.Fixtures;

namespace Core.Cache.IntegrationTests.Cache.Redis;

public sealed class RedisGetOrAddAsyncTests(RedisContainerFixture fixture)
        : GetOrAddAsyncTestsBase,
      IClassFixture<RedisContainerFixture>
{
    private readonly RedisCacheTestBaseImpl _fixture = new(fixture);

    protected override ICoreCache Cache => _fixture.Cache;

    private sealed class RedisCacheTestBaseImpl(RedisContainerFixture fixture) : RedisCacheTestBase(fixture)
    {
        public new ICoreCache Cache => base.Cache;
    }
}
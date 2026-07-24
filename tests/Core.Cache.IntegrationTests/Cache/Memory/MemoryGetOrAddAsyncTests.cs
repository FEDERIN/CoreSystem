using Core.Cache.Abstractions;
using Core.Cache.IntegrationTests.Fixtures;

namespace Core.Cache.IntegrationTests.Cache.Memory;

public sealed class MemoryGetOrAddAsyncTests
    : GetOrAddAsyncTestsBase
{
    private readonly MemoryCacheTestBaseImpl _fixture = new();

    protected override ICoreCache Cache => _fixture.Cache;

    private sealed class MemoryCacheTestBaseImpl : MemoryCacheTestBase
    {
        public new ICoreCache Cache => base.Cache;
    }
}
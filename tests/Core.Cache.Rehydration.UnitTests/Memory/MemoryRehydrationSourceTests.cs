using Core.Cache.Rehydration.Memory;
using Core.Cache.Storage;
using Core.Cache.Storage.Abstractions;
using Core.Cache.Storage.Memory;
using Microsoft.Extensions.Caching.Memory;
using ICacheEntry = Core.Cache.Storage.Abstractions.ICacheEntry;

namespace Core.Cache.Rehydration.UnitTests.Memory;

public sealed class MemoryRehydrationSourceTests
{
    [Fact]
    public void GetEntries_ReturnsTrackedEntry()
    {
        using var memoryCache = new MemoryCache(
            new MemoryCacheOptions());

        var tracker = new FakeCacheKeyTracker();
        var inspector = new CacheEntryInspector();

        const string key = "user:123";
        const string value = "John Doe";

        var wrapper = new CacheEntryWrapper<string>
        {
            Value = value,
            Tags = ["users"]
        };

        memoryCache.Set(key, wrapper);
        tracker.Track(key);

        var sut = new MemoryRehydrationSource(
            memoryCache,
            tracker,
            inspector);

        var entries = sut.GetEntries().ToList();

        var entry = Assert.Single(entries);

        Assert.Equal(key, entry.Key);
        Assert.Equal(value, entry.Value);
        Assert.Null(entry.RemainingExpiration);
        Assert.Equal(["users"], entry.Tags);
    }

    [Fact]
    public void GetEntries_SkipsEntry_WhenMemoryEntryDoesNotExist()
    {
        using var memoryCache = new MemoryCache(
            new MemoryCacheOptions());

        var tracker = new FakeCacheKeyTracker();
        var inspector = new CacheEntryInspector();

        tracker.Track("missing-key");

        var sut = new MemoryRehydrationSource(
            memoryCache,
            tracker,
            inspector);

        var entries = sut.GetEntries().ToList();

        Assert.Empty(entries);
    }

    [Fact]
    public void GetEntries_SkipsEntry_WhenInspectorCannotGetWrapper()
    {
        using var memoryCache = new MemoryCache(
            new MemoryCacheOptions());

        var tracker = new FakeCacheKeyTracker();
        var inspector = new FakeCacheEntryInspector
        {
            CanInspect = false
        };

        const string key = "user:123";

        memoryCache.Set(
            key,
            new object());

        tracker.Track(key);

        var sut = new MemoryRehydrationSource(
            memoryCache,
            tracker,
            inspector);

        var entries = sut.GetEntries().ToList();

        Assert.Empty(entries);
    }

    [Fact]
    public void GetEntries_SkipsEntry_WhenInspectorReturnsNullWrapper()
    {
        using var memoryCache = new MemoryCache(
            new MemoryCacheOptions());

        var tracker = new FakeCacheKeyTracker();
        var inspector = new FakeCacheEntryInspector
        {
            CanInspect = true,
            Wrapper = null
        };

        const string key = "user:123";

        memoryCache.Set(
            key,
            new object());

        tracker.Track(key);

        var sut = new MemoryRehydrationSource(
            memoryCache,
            tracker,
            inspector);

        var entries = sut.GetEntries().ToList();

        Assert.Empty(entries);
    }

    [Fact]
    public void GetEntries_ReturnsRemainingExpiration()
    {
        using var memoryCache = new MemoryCache(
            new MemoryCacheOptions());

        var tracker = new FakeCacheKeyTracker();
        var inspector = new CacheEntryInspector();

        const string key = "user:123";

        var expiration = TimeSpan.FromMinutes(5);

        var wrapper = new CacheEntryWrapper<string>
        {
            Value = "Federin",
            AbsoluteExpiration =
                DateTimeOffset.UtcNow.Add(expiration)
        };

        memoryCache.Set(key, wrapper);
        tracker.Track(key);

        var sut = new MemoryRehydrationSource(
            memoryCache,
            tracker,
            inspector);

        var entries = sut.GetEntries().ToList();

        var entry = Assert.Single(entries);

        Assert.NotNull(entry.RemainingExpiration);
        Assert.True(
            entry.RemainingExpiration <= expiration);
        Assert.True(
            entry.RemainingExpiration > TimeSpan.Zero);
    }

    [Fact]
    public void GetEntries_SkipsEntry_WhenEntryIsExpired()
    {
        using var memoryCache = new MemoryCache(
            new MemoryCacheOptions());

        var tracker = new FakeCacheKeyTracker();
        var inspector = new CacheEntryInspector();

        const string key = "user:123";

        var wrapper = new CacheEntryWrapper<string>
        {
            Value = "Federin",
            AbsoluteExpiration =
                DateTimeOffset.UtcNow.AddSeconds(-1)
        };

        memoryCache.Set(key, wrapper);
        tracker.Track(key);

        var sut = new MemoryRehydrationSource(
            memoryCache,
            tracker,
            inspector);

        var entries = sut.GetEntries().ToList();

        Assert.Empty(entries);
    }

    [Fact]
    public void GetEntries_PreservesTags()
    {
        using var memoryCache = new MemoryCache(
            new MemoryCacheOptions());

        var tracker = new FakeCacheKeyTracker();
        var inspector = new CacheEntryInspector();

        const string key = "user:123";

        var tags = new[]
        {
            "users",
            "premium",
            "south-america"
        };

        var wrapper = new CacheEntryWrapper<string>
        {
            Value = "Federin",
            Tags = tags
        };

        memoryCache.Set(key, wrapper);
        tracker.Track(key);

        var sut = new MemoryRehydrationSource(
            memoryCache,
            tracker,
            inspector);

        var entries = sut.GetEntries().ToList();

        var entry = Assert.Single(entries);

        Assert.NotNull(entry.Tags);
        Assert.Equal(tags, entry.Tags);
    }

    [Fact]
    public async Task RemoveForRehydrationAsync_RemovesEntryFromMemoryCache()
    {
        using var memoryCache = new MemoryCache(
            new MemoryCacheOptions());

        var tracker = new FakeCacheKeyTracker();
        var inspector = new CacheEntryInspector();

        const string key = "user:123";

        var wrapper = new CacheEntryWrapper<string>
        {
            Value = "John Doe"
        };

        memoryCache.Set(key, wrapper);

        var sut = new MemoryRehydrationSource(
            memoryCache,
            tracker,
            inspector);

        await sut.RemoveForRehydrationAsync(key, TestContext.Current.CancellationToken);

        Assert.False(
            memoryCache.TryGetValue(
                key,
                out _));
    }

    private sealed class FakeCacheKeyTracker
        : ICacheKeyTracker
    {
        private readonly List<string> _keys = [];

        public void Track(string key)
        {
            _keys.Add(key);
        }

        public void Untrack(string key)
        {
            _keys.Remove(key);
        }

        public IEnumerable<string> GetAllTrackedKeys()
        {
            return _keys;
        }
    }

    private sealed class FakeCacheEntryInspector
        : ICacheEntryInspector
    {
        public bool CanInspect { get; init; }

        public ICacheEntry? Wrapper { get; init; }

        public bool TryGet(
            object? entry,
            out ICacheEntry? wrapper)
        {
            wrapper = Wrapper;

            return CanInspect;
        }

        public bool TryGetValue<T>(
            object? entry,
            out T? value)
        {
            value = default;

            return false;
        }
    }
}
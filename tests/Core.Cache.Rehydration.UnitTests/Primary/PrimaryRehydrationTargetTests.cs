using Core.Cache.Abstractions;
using Core.Cache.Rehydration.Abstractions;
using Core.Cache.Rehydration.Primary;
using Core.Cache.Storage;

namespace Core.Cache.Rehydration.UnitTests.Primary;

public sealed class PrimaryRehydrationTargetTests
{
    [Fact]
    public async Task StoreAsync_StoresEntryInPrimary()
    {
        var primary = new FakeCacheStorage();
        var resolver = new FakeCacheStorageResolver(primary);

        var sut = new PrimaryRehydrationTarget(resolver);

        var entry = new CacheRehydrationEntry
        {
            Key = "user:123",
            Value = "Jhon Doe"
        };

        await sut.StoreAsync(entry, TestContext.Current.CancellationToken);

        Assert.True(primary.SetCalled);
        Assert.Equal("user:123", primary.Key);
        Assert.Equal("Jhon Doe", primary.Value);
    }

    [Fact]
    public async Task StoreAsync_PassesRemainingExpirationToPrimary()
    {
        var primary = new FakeCacheStorage();
        var resolver = new FakeCacheStorageResolver(primary);

        var sut = new PrimaryRehydrationTarget(resolver);

        var expiration = TimeSpan.FromMinutes(5);

        var entry = new CacheRehydrationEntry
        {
            Key = "user:123",
            Value = "Jhon Doe",
            RemainingExpiration = expiration
        };

        await sut.StoreAsync(entry, TestContext.Current.CancellationToken);

        Assert.Equal(
            expiration,
            primary.Expiration);
    }

    [Fact]
    public async Task StoreAsync_PassesTagsToPrimary()
    {
        var primary = new FakeCacheStorage();
        var resolver = new FakeCacheStorageResolver(primary);

        var sut = new PrimaryRehydrationTarget(resolver);

        var entry = new CacheRehydrationEntry
        {
            Key = "user:123",
            Value = "Jhon Doe",
            Tags =
            [
                "users",
                "premium"
            ]
        };

        await sut.StoreAsync(entry, TestContext.Current.CancellationToken);

        Assert.NotNull(primary.Tags);
        Assert.Equal(
            ["users", "premium"],
            primary.Tags);
    }

    [Fact]
    public async Task StoreAsync_PassesNullTagsToPrimary_WhenEntryHasNoTags()
    {
        var primary = new FakeCacheStorage();
        var resolver = new FakeCacheStorageResolver(primary);

        var sut = new PrimaryRehydrationTarget(resolver);

        var entry = new CacheRehydrationEntry
        {
            Key = "user:123",
            Value = "Jhon Doe",
            Tags = null
        };

        await sut.StoreAsync(entry, TestContext.Current.CancellationToken);

        Assert.Null(primary.Tags);
    }

    [Fact]
    public async Task StoreAsync_PassesCancellationTokenToPrimary()
    {
        var primary = new FakeCacheStorage();
        var resolver = new FakeCacheStorageResolver(primary);

        var sut = new PrimaryRehydrationTarget(resolver);

        using var cancellationTokenSource =
            new CancellationTokenSource();

        var cancellationToken =
            cancellationTokenSource.Token;

        var entry = new CacheRehydrationEntry
        {
            Key = "user:123",
            Value = "Jhon Doe"
        };

        await sut.StoreAsync(
            entry,
            cancellationToken);

        Assert.Equal(
            cancellationToken,
            primary.CancellationToken);
    }

    private sealed class FakeCacheStorageResolver(
        ICacheStorage primary)
        : ICacheStorageResolver
    {
        public ICacheStorage Primary { get; } = primary;

        public ICacheStorage? Fallback => null;

        public bool HasFallback => false;
    }

    private sealed class FakeCacheStorage : ICacheStorage
    {
        public bool SetCalled { get; private set; }

        public string? Key { get; private set; }

        public object? Value { get; private set; }

        public TimeSpan? Expiration { get; private set; }

        public string[]? Tags { get; private set; }

        public CancellationToken CancellationToken { get; private set; }

        public Task<T?> GetAsync<T>(
            string key,
            CancellationToken ct = default)
        {
            return Task.FromResult<T?>(default);
        }

        public Task SetAsync<T>(
            string key,
            T value,
            CacheEntryOptions? options = null,
            TimeSpan? expiration = null,
            string[]? tags = null,
            CancellationToken ct = default)
        {
            SetCalled = true;
            Key = key;
            Value = value;
            Expiration = expiration;
            Tags = tags;
            CancellationToken = ct;

            return Task.CompletedTask;
        }

        public Task RemoveAsync(
            string key,
            CancellationToken ct = default)
        {
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(
            string key,
            CancellationToken ct = default)
        {
            return Task.FromResult(false);
        }

        public Task InvalidateByTagAsync(
            string tag,
            CancellationToken ct = default)
        {
            return Task.CompletedTask;
        }

        public Task<T?> GetOrAddAsync<T>(
            string key,
            Func<CancellationToken, Task<T>> factory,
            CacheEntryOptions? options = null,
            TimeSpan? expiration = null,
            string[]? tags = null,
            CancellationToken ct = default)
        {
            return Task.FromResult<T?>(default);
        }
    }
}
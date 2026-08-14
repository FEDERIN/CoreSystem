using Core.Cache.Redis.Builders;
using Core.Cache.Redis.Storage;
using Moq;
using StackExchange.Redis;

namespace Core.Cache.Redis.UnitTests.Storage;

public sealed class RedisTagIndexTests
{
    private readonly Mock<IConnectionMultiplexer> _redis;
    private readonly Mock<IDatabase> _database;
    private readonly Mock<IKeyBuilder> _keyBuilder;

    private readonly RedisTagIndex _sut;

    public RedisTagIndexTests()
    {
        _redis = new Mock<IConnectionMultiplexer>();
        _database = new Mock<IDatabase>();
        _keyBuilder = new Mock<IKeyBuilder>();

        _redis
            .Setup(x => x.GetDatabase(
                It.IsAny<int>(),
                It.IsAny<object?>()))
            .Returns(_database.Object);

        _keyBuilder
            .Setup(x => x.BuildCacheKey(It.IsAny<string>()))
            .Returns((string key) => $"cache:{key}");

        _keyBuilder
            .Setup(x => x.BuildTag(It.IsAny<string>()))
            .Returns((string tag) => $"cache:tag:{tag}");

        _keyBuilder
            .Setup(x => x.BuildTagsIndex(It.IsAny<string>()))
            .Returns((string key) => $"cache:{key}:tags");

        _sut = new RedisTagIndex(
            _redis.Object,
            _keyBuilder.Object);
    }

    [Fact]
    public async Task AddAsync_AddsKeyToTagAndTagToKeyIndex()
    {
        _database
            .Setup(x => x.SetAddAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        await _sut.AddAsync(
            "user:1",
            ["users"],
            TestContext.Current.CancellationToken);

        _database.Verify(
            x => x.SetAddAsync(
                "cache:tag:users",
                "user:1",
                CommandFlags.None),
            Times.Once);

        _database.Verify(
            x => x.SetAddAsync(
                "cache:user:1:tags",
                "users",
                CommandFlags.None),
            Times.Once);
    }

    [Fact]
    public async Task AddAsync_AddsBothIndexesForEveryTag()
    {
        _database
            .Setup(x => x.SetAddAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        await _sut.AddAsync(
            "user:1",
            ["users", "premium"],
            TestContext.Current.CancellationToken);

        _database.Verify(
            x => x.SetAddAsync(
                "cache:tag:users",
                "user:1",
                CommandFlags.None),
            Times.Once);

        _database.Verify(
            x => x.SetAddAsync(
                "cache:user:1:tags",
                "users",
                CommandFlags.None),
            Times.Once);

        _database.Verify(
            x => x.SetAddAsync(
                "cache:tag:premium",
                "user:1",
                CommandFlags.None),
            Times.Once);

        _database.Verify(
            x => x.SetAddAsync(
                "cache:user:1:tags",
                "premium",
                CommandFlags.None),
            Times.Once);
    }

    [Fact]
    public async Task AddAsync_ThrowsOperationCanceledException_WhenCancellationIsRequested()
    {
        using var cancellationTokenSource = new CancellationTokenSource();

        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _sut.AddAsync(
                "user:1",
                ["users"],
                cancellationTokenSource.Token));

        _database.Verify(
            x => x.SetAddAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                It.IsAny<CommandFlags>()),
            Times.Never);
    }

    [Fact]
    public async Task RemoveKeyAsync_DoesNothing_WhenKeyHasNoTags()
    {
        _database
            .Setup(x => x.SetMembersAsync(
                "cache:user:1:tags",
                CommandFlags.None))
            .ReturnsAsync([]);

        await _sut.RemoveKeyAsync(
            "user:1",
            TestContext.Current.CancellationToken);

        _database.Verify(
            x => x.SetMembersAsync(
                "cache:user:1:tags",
                CommandFlags.None),
            Times.Once);

        _database.Verify(
            x => x.SetRemoveAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                It.IsAny<CommandFlags>()),
            Times.Never);

        _database.Verify(
            x => x.KeyDeleteAsync(
                "cache:user:1:tags",
                CommandFlags.None),
            Times.Never);
    }

    [Fact]
    public async Task RemoveKeyAsync_RemovesKeyFromEveryTagAndDeletesKeyIndex()
    {
        RedisValue[] tags =
        [
            "users",
        "premium"
        ];

        _database
            .Setup(x => x.SetMembersAsync(
                "cache:user:1:tags",
                CommandFlags.None))
            .ReturnsAsync(tags);

        _database
            .Setup(x => x.SetRemoveAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        _database
            .Setup(x => x.KeyDeleteAsync(
                "cache:user:1:tags",
                CommandFlags.None))
            .ReturnsAsync(true);

        await _sut.RemoveKeyAsync(
            "user:1",
            TestContext.Current.CancellationToken);

        _database.Verify(
            x => x.SetRemoveAsync(
                "cache:tag:users",
                "user:1",
                CommandFlags.None),
            Times.Once);

        _database.Verify(
            x => x.SetRemoveAsync(
                "cache:tag:premium",
                "user:1",
                CommandFlags.None),
            Times.Once);

        _database.Verify(
            x => x.KeyDeleteAsync(
                "cache:user:1:tags",
                CommandFlags.None),
            Times.Once);
    }

    [Fact]
    public async Task RemoveKeyAsync_ThrowsOperationCanceledException_WhenCancellationIsRequested()
    {
        using var cancellationTokenSource = new CancellationTokenSource();

        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _sut.RemoveKeyAsync(
                "user:1",
                cancellationTokenSource.Token));

        _database.Verify(
            x => x.SetMembersAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<CommandFlags>()),
            Times.Never);
    }

    [Fact]
    public async Task InvalidateTagAsync_DoesNothing_WhenTagHasNoKeys()
    {
        _database
            .Setup(x => x.SetMembersAsync(
                "cache:tag:users",
                CommandFlags.None))
            .ReturnsAsync([]);

        var removeEntry =
            new Mock<Func<string, CancellationToken, Task>>();

        await _sut.InvalidateTagAsync(
            "users",
            removeEntry.Object,
            TestContext.Current.CancellationToken);

        removeEntry.Verify(
            x => x(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        _database.Verify(
            x => x.KeyDeleteAsync(
                "cache:tag:users",
                CommandFlags.None),
            Times.Never);
    }

    [Fact]
    public async Task InvalidateTagAsync_RemovesAllEntriesAndDeletesTagIndex()
    {
        RedisValue[] members =
        [
            "user:1",
        "user:2"
        ];

        _database
            .Setup(x => x.SetMembersAsync(
                "cache:tag:users",
                CommandFlags.None))
            .ReturnsAsync(members);

        var removedKeys = new List<string>();

        Task RemoveEntry(
            string key,
            CancellationToken _)
        {
            removedKeys.Add(key);

            return Task.CompletedTask;
        }

        await _sut.InvalidateTagAsync(
            "users",
            RemoveEntry,
            TestContext.Current.CancellationToken);

        Assert.Equal(
            ["user:1", "user:2"],
            removedKeys);

        _database.Verify(
            x => x.KeyDeleteAsync(
                "cache:tag:users",
                CommandFlags.None),
            Times.Once);
    }

    [Fact]
    public async Task GetKeysAsync_ReturnsKeysAssociatedWithTag()
    {
        RedisValue[] members =
        [
            "user:1",
        "user:2"
        ];

        _database
            .Setup(x => x.SetMembersAsync(
                "cache:tag:users",
                CommandFlags.None))
            .ReturnsAsync(members);

        var result =
            await _sut.GetKeysAsync(
                "users",
                TestContext.Current.CancellationToken);

        Assert.Equal(
            ["user:1", "user:2"],
            result);
    }

    [Fact]
    public async Task CountAsync_ReturnsNumberOfKeysForTag()
    {
        _database
            .Setup(x => x.SetLengthAsync(
                "cache:tag:users",
                CommandFlags.None))
            .ReturnsAsync(3);

        var result =
            await _sut.CountAsync(
                "users",
                TestContext.Current.CancellationToken);

        Assert.Equal(3, result);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsTrue_WhenTagExists()
    {
        _database
            .Setup(x => x.KeyExistsAsync(
                "cache:tag:users",
                CommandFlags.None))
            .ReturnsAsync(true);

        var result =
            await _sut.ExistsAsync(
                "users",
                TestContext.Current.CancellationToken);

        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsFalse_WhenTagDoesNotExist()
    {
        _database
            .Setup(x => x.KeyExistsAsync(
                "cache:tag:users",
                CommandFlags.None))
            .ReturnsAsync(false);

        var result =
            await _sut.ExistsAsync(
                "users",
                TestContext.Current.CancellationToken);

        Assert.False(result);
    }

    [Fact]
    public async Task GetKeysAsync_ThrowsOperationCanceledException_WhenCancellationIsRequested()
    {
        using var cts = new CancellationTokenSource();

        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _sut.GetKeysAsync(
                "users",
                cts.Token));

        _database.Verify(
            x => x.SetMembersAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<CommandFlags>()),
            Times.Never);
    }

    [Fact]
    public async Task CountAsync_ThrowsOperationCanceledException_WhenCancellationIsRequested()
    {
        using var cts = new CancellationTokenSource();

        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _sut.CountAsync(
                "users",
                cts.Token));

        _database.Verify(
            x => x.SetLengthAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<CommandFlags>()),
            Times.Never);
    }

    [Fact]
    public async Task ExistsAsync_ThrowsOperationCanceledException_WhenCancellationIsRequested()
    {
        using var cts = new CancellationTokenSource();

        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _sut.ExistsAsync(
                "users",
                cts.Token));

        _database.Verify(
            x => x.KeyExistsAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<CommandFlags>()),
            Times.Never);
    }
}
using Core.Cache.Exceptions;
using Core.Cache.Redis.Builders;
using Core.Cache.Redis.Storage;
using Core.Cache.Storage;
using Core.Cache.Storage.Abstractions;
using Core.Redis.Synchronization;
using Core.Serialization;
using Core.Serialization.Abstractions;
using Microsoft.Extensions.Logging;
using Moq;
using StackExchange.Redis;

namespace Core.Cache.Redis.UnitTests.Storage;

public sealed class RedisCacheStorageTests
{
    private readonly Mock<IConnectionMultiplexer> _redis;
    private readonly Mock<IDatabase> _database;
    private readonly Mock<IPayloadSerializer> _serializer;
    private readonly Mock<IKeyBuilder> _keyBuilder;
    private readonly Mock<ICacheTagIndex<RedisCacheStorage>> _tagIndex;
    private readonly Mock<IDistributedLockProvider> _lockProvider;
    private readonly Mock<ILogger<RedisCacheStorage>> _logger;

    private readonly RedisCacheStorage _sut;

    public RedisCacheStorageTests()
    {
        _redis = new Mock<IConnectionMultiplexer>();
        _database = new Mock<IDatabase>();
        _serializer = new Mock<IPayloadSerializer>();
        _keyBuilder = new Mock<IKeyBuilder>();
        _tagIndex = new Mock<ICacheTagIndex<RedisCacheStorage>>();
        _lockProvider = new Mock<IDistributedLockProvider>();
        _logger = new Mock<ILogger<RedisCacheStorage>>();

        _redis
            .Setup(x => x.GetDatabase(
                It.IsAny<int>(),
                It.IsAny<object?>()))
            .Returns(_database.Object);

        _keyBuilder
            .Setup(x => x.BuildCacheKey(It.IsAny<string>()))
            .Returns((string key) => $"cache:{key}");

        _keyBuilder
            .Setup(x => x.BuildLock(It.IsAny<string>()))
            .Returns((string key) => $"cache:{key}:lock");

        _sut = new RedisCacheStorage(
            _redis.Object,
            _serializer.Object,
            _keyBuilder.Object,
            _tagIndex.Object,
            _lockProvider.Object,
            _logger.Object);
    }

    [Fact]
    public async Task GetAsync_ReturnsDefault_WhenKeyDoesNotExist()
    {
        _database
            .Setup(x => x.StringGetAsync(
                "cache:user:1",
                CommandFlags.None))
            .ReturnsAsync(RedisValue.Null);

        var result =
            await _sut.GetAsync<string>("user:1", TestContext.Current.CancellationToken);

        Assert.Null(result);

        _database.Verify(
            x => x.KeyDeleteAsync(
                "cache:user:1",
                CommandFlags.None),
            Times.Never);
    }

    [Fact]
    public async Task GetAsync_ReturnsDeserializedValue_WhenKeyExists()
    {
        var payload = new byte[] { 1, 2, 3 };

        _database
            .Setup(x => x.StringGetAsync(
                "cache:user:1",
                CommandFlags.None))
            .ReturnsAsync((RedisValue)payload);

        _serializer
            .Setup(x => x.Deserialize<string>(
                It.IsAny<ReadOnlyMemory<byte>>()))
            .Callback<ReadOnlyMemory<byte>>(
                value =>
                {
                    Assert.False(value.IsEmpty);
                })
            .Returns("John");

        var result =
            await _sut.GetAsync<string>("user:1", TestContext.Current.CancellationToken);

        _serializer.Verify(
            x => x.Deserialize<string>(
                It.IsAny<ReadOnlyMemory<byte>>()),
            Times.Once);

        Assert.Equal("John", result);
    }

    [Fact]
    public async Task GetAsync_ReturnsDefaultAndDeletesKey_WhenDeserializationFails()
    {
        var payload = new byte[] { 1, 2, 3 };

        _database
            .Setup(x => x.StringGetAsync(
                "cache:user:1",
                CommandFlags.None))
            .ReturnsAsync((RedisValue)payload);

        _serializer
            .Setup(x => x.Deserialize<string>(
                It.IsAny<ReadOnlyMemory<byte>>()))
            .Throws(
                new CacheDeserializationException(
                    SerializerType.Json,
                    "Invalid payload.",
                    new InvalidOperationException()));

        var result =
            await _sut.GetAsync<string>("user:1", TestContext.Current.CancellationToken);

        Assert.Null(result);

        _database.Verify(
            x => x.KeyDeleteAsync(
                "cache:user:1",
                CommandFlags.None),
            Times.Once);
    }

    [Fact]
    public async Task SetAsync_SerializesAndStoresValue()
    {
        var payload = new byte[] { 1, 2, 3 };

        _serializer
            .Setup(x => x.Serialize("John"))
            .Returns(payload);

        _database
            .Setup(x => x.StringSetAsync(
                "cache:user:1",
                payload,
                It.IsAny<Expiration>(),
                It.IsAny<ValueCondition>(),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        await _sut.SetAsync(
            "user:1",
            "John",
            ct: TestContext.Current.CancellationToken);

        _serializer.Verify(
            x => x.Serialize("John"),
            Times.Once);

        _database.Verify(
            x => x.StringSetAsync(
                "cache:user:1",
                payload,
                It.IsAny<Expiration>(),
                It.IsAny<ValueCondition>(),
                It.IsAny<CommandFlags>()),
            Times.Once);

        _tagIndex.Verify(
            x => x.AddAsync(
                It.IsAny<string>(),
                It.IsAny<IReadOnlyCollection<string>>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task SetAsync_AddsTags_WhenTagsAreProvided()
    {
        var payload = new byte[] { 1, 2, 3 };

        string[] tags =
        [
            "users",
        "premium"
        ];

        _serializer
            .Setup(x => x.Serialize("John"))
            .Returns(payload);

        _database
            .Setup(x => x.StringSetAsync(
                It.IsAny<RedisKey>(),
                It.IsAny<RedisValue>(),
                It.IsAny<Expiration>(),
                It.IsAny<ValueCondition>(),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        await _sut.SetAsync(
            "user:1",
            "John",
            tags: tags,
            ct: TestContext.Current.CancellationToken);

        _tagIndex.Verify(
            x => x.AddAsync(
                "user:1",
                It.Is<IReadOnlyCollection<string>>(
                    x => x.SequenceEqual(tags)),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task RemoveAsync_DeletesRedisKeyAndRemovesTags()
    {
        await _sut.RemoveAsync("user:1", TestContext.Current.CancellationToken);

        _database.Verify(
            x => x.KeyDeleteAsync(
                "cache:user:1",
                It.IsAny<CommandFlags>()),
            Times.Once);

        _tagIndex.Verify(
            x => x.RemoveKeyAsync(
                "user:1",
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsTrue_WhenKeyExists()
    {
        _database
            .Setup(x => x.KeyExistsAsync(
                "cache:user:1",
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        var result =
            await _sut.ExistsAsync("user:1", TestContext.Current.CancellationToken);

        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsFalse_WhenKeyDoesNotExist()
    {
        _database
            .Setup(x => x.KeyExistsAsync(
                "cache:user:1",
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(false);

        var result =
            await _sut.ExistsAsync("user:1", TestContext.Current.CancellationToken);

        Assert.False(result);
    }

    [Fact]
    public async Task InvalidateByTagAsync_DeletesKeysReturnedByTagIndex()
    {
        Func<string, CancellationToken, Task>? removeEntry = null;

        _tagIndex
            .Setup(x => x.InvalidateTagAsync(
                "users",
                It.IsAny<Func<string, CancellationToken, Task>>(),
                It.IsAny<CancellationToken>()))
            .Callback<string, Func<string, CancellationToken, Task>, CancellationToken>(
                (_, callback, _) =>
                {
                    removeEntry = callback;
                })
            .Returns(Task.CompletedTask);

        await _sut.InvalidateByTagAsync("users", TestContext.Current.CancellationToken);

        Assert.NotNull(removeEntry);

        await removeEntry!(
            "user:1",
            TestContext.Current.CancellationToken);

        _database.Verify(
            x => x.KeyDeleteAsync(
                "user:1",
                It.IsAny<CommandFlags>()),
            Times.Once);
    }

    [Fact]
    public async Task GetOrAddAsync_ReturnsCachedValue_WithoutExecutingFactory()
    {
        var payload = new byte[] { 1, 2, 3 };

        _database
            .Setup(x => x.StringGetAsync(
                "cache:user:1",
                CommandFlags.None))
            .ReturnsAsync((RedisValue)payload);

        _serializer
            .Setup(x => x.Deserialize<string>(
                It.IsAny<ReadOnlyMemory<byte>>()))
            .Returns("John");

        var factoryCalled = false;

        var result =
            await _sut.GetOrAddAsync(
                "user:1",
                _ =>
                {
                    factoryCalled = true;

                    return Task.FromResult("Generated");
                }, 
                ct: TestContext.Current.CancellationToken);

        Assert.Equal("John", result);
        Assert.False(factoryCalled);

        _lockProvider.Verify(
            x => x.AcquireAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetOrAddAsync_DoesNotExecuteFactory_WhenValueAppearsAfterLock()
    {
        var lockHandle = new Mock<IDisposable>();

        var payload = new byte[] { 1, 2, 3 };

        _database
            .SetupSequence(x => x.StringGetAsync(
                "cache:user:1",
                CommandFlags.None))
            .ReturnsAsync(RedisValue.Null)
            .ReturnsAsync((RedisValue)payload);

        _serializer
            .Setup(x => x.Deserialize<string>(
                It.IsAny<ReadOnlyMemory<byte>>()))
            .Returns("John");

        _lockProvider
            .Setup(x => x.AcquireAsync(
                "cache:user:1:lock",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(lockHandle.Object);

        var factory = new Mock<Func<CancellationToken, Task<string>>>();

        var result =
            await _sut.GetOrAddAsync(
                "user:1",
                factory.Object,
                ct: TestContext.Current.CancellationToken);

        Assert.Equal("John", result);

        factory.Verify(
            x => x(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    //[Fact]
    //public async Task GetOrAddAsync_DoesNotStoreValue_WhenFactoryReturnsNull()
    //{
    //    var lockHandle = new Mock<IDisposable>();

    //    _database
    //        .SetupSequence(x => x.StringGetAsync(
    //            "cache:user:1",
    //            It.IsAny<CommandFlags>()))
    //        .ReturnsAsync(RedisValue.Null)
    //        .ReturnsAsync(RedisValue.Null);

    //    _lockProvider
    //        .Setup(x => x.AcquireAsync(
    //            "cache:user:1:lock",
    //            It.IsAny<CancellationToken>()))
    //        .ReturnsAsync(lockHandle.Object);

    //    var result =
    //        await _sut.GetOrAddAsync<string>(
    //            "user:1",
    //            _ => Task.FromResult<string?>(null),
    //            ct: TestContext.Current.CancellationToken);

    //    Assert.Null(result);

    //    _serializer.Verify(
    //        x => x.Serialize(
    //            It.IsAny<string>()),
    //        Times.Never);

    //    _database.Verify(
    //        x => x.StringSetAsync(
    //            It.IsAny<RedisKey>(),
    //            It.IsAny<RedisValue>(),
    //            It.IsAny<Expiration>(),
    //            It.IsAny<When>(),
    //            It.IsAny<CommandFlags>()),
    //        Times.Never);
    //}

    [Fact]
    public async Task GetOrAddAsync_PassesCancellationTokenToLock()
    {
        _database
            .Setup(x => x.StringGetAsync(
                "cache:user:1",
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(RedisValue.Null);

        _lockProvider
            .Setup(x => x.AcquireAsync(
                "cache:user:1:lock",
                TestContext.Current.CancellationToken))
            .ThrowsAsync(
                new OperationCanceledException(TestContext.Current.CancellationToken));

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _sut.GetOrAddAsync(
                "user:1",
                _ => Task.FromResult("John"),
                ct: TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task GetOrAddAsync_ExecutesFactoryAndStoresValue_WhenValueIsMissing()
    {
        var lockHandle = new Mock<IDisposable>();

        var payload = new byte[] { 1, 2, 3 };

        _database
            .SetupSequence(x => x.StringGetAsync(
                "cache:user:1",
                CommandFlags.None))
            .ReturnsAsync(RedisValue.Null)
            .ReturnsAsync(RedisValue.Null);

        _lockProvider
            .Setup(x => x.AcquireAsync(
                "cache:user:1:lock",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(lockHandle.Object);

        _serializer
            .Setup(x => x.Serialize("John"))
            .Returns(payload);

        _database
            .Setup(x => x.StringSetAsync(
                "cache:user:1",
                payload,
                It.IsAny<Expiration>(),
                It.IsAny<ValueCondition>(),
                It.IsAny<CommandFlags>()))
            .ReturnsAsync(true);

        var result =
            await _sut.GetOrAddAsync(
                "user:1",
                _ => Task.FromResult("John"),
                ct: TestContext.Current.CancellationToken);

        Assert.Equal("John", result);

        _serializer.Verify(
            x => x.Serialize("John"),
            Times.Once);

        _database.Verify(
            x => x.StringSetAsync(
                "cache:user:1",
                payload,
                It.IsAny<Expiration>(),
                It.IsAny<ValueCondition>(),
                It.IsAny<CommandFlags>()),
            Times.Once);
    }
}
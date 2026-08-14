using Core.Cache.Redis.Builders;

namespace Core.Cache.Redis.UnitTests.Builders;

public sealed class RedisKeyBuilderTests
{
    [Fact]
    public void BuildCacheKey_ReturnsKeyWithPrefix()
    {
        var sut = new RedisKeyBuilder("cache:");

        var result = sut.BuildCacheKey("user:1");

        Assert.Equal(
            "cache:user:1",
            result);
    }

    [Fact]
    public void BuildCacheKey_ReturnsKeyWithoutPrefix_WhenPrefixIsEmpty()
    {
        var sut = new RedisKeyBuilder(string.Empty);

        var result = sut.BuildCacheKey("user:1");

        Assert.Equal(
            "user:1",
            result);
    }

    [Fact]
    public void BuildTag_ReturnsTagKeyWithPrefix()
    {
        var sut = new RedisKeyBuilder("cache:");

        var result = sut.BuildTag("users");

        Assert.Equal(
            "cache:tag:users",
            result);
    }

    [Fact]
    public void BuildLock_ReturnsLockKey()
    {
        var sut = new RedisKeyBuilder("cache:");

        var result = sut.BuildLock("user:1");

        Assert.Equal(
            "cache:user:1:lock",
            result);
    }

    [Fact]
    public void BuildTagsIndex_ReturnsTagsIndexKey()
    {
        var sut = new RedisKeyBuilder("cache:");

        var result = sut.BuildTagsIndex("user:1");

        Assert.Equal(
            "cache:user:1:tags",
            result);
    }
}
using Core.Cache.Redis.Diagnostics;

namespace Core.Cache.Redis.UnitTests.Diagnostics;

public sealed class RedisHealthStateTests
{
    [Fact]
    public void IsRedisHealthy_IsTrue_ByDefault()
    {
        var sut = new RedisHealthState();

        Assert.True(sut.IsRedisHealthy);
    }

    [Fact]
    public void Update_ReturnsNone_WhenStateDoesNotChange()
    {
        var sut = new RedisHealthState();

        var result = sut.Update(true);

        Assert.Equal(
            HealthTransition.None,
            result);

        Assert.True(sut.IsRedisHealthy);
    }

    [Fact]
    public void Update_ReturnsBecameUnhealthy_WhenRedisBecomesUnavailable()
    {
        var sut = new RedisHealthState();

        var result = sut.Update(false);

        Assert.Equal(
            HealthTransition.BecameUnhealthy,
            result);

        Assert.False(sut.IsRedisHealthy);
    }

    [Fact]
    public void Update_ReturnsNone_WhenRedisRemainsUnavailable()
    {
        var sut = new RedisHealthState();

        sut.Update(false);

        var result = sut.Update(false);

        Assert.Equal(
            HealthTransition.None,
            result);

        Assert.False(sut.IsRedisHealthy);
    }

    [Fact]
    public void Update_ReturnsBecameHealthy_WhenRedisRecovers()
    {
        var sut = new RedisHealthState();

        sut.Update(false);

        var result = sut.Update(true);

        Assert.Equal(
            HealthTransition.BecameHealthy,
            result);

        Assert.True(sut.IsRedisHealthy);
    }

    [Fact]
    public void Update_ReturnsNone_WhenRedisRemainsHealthy()
    {
        var sut = new RedisHealthState();

        sut.Update(true);

        var result = sut.Update(true);

        Assert.Equal(
            HealthTransition.None,
            result);

        Assert.True(sut.IsRedisHealthy);
    }

    [Fact]
    public void MarkUnavailable_SetsRedisAsUnhealthy()
    {
        var sut = new RedisHealthState();

        sut.MarkUnavailable();

        Assert.False(sut.IsRedisHealthy);
    }

    [Fact]
    public void MarkUnavailable_DoesNotChangeAlreadyUnhealthyState()
    {
        var sut = new RedisHealthState();

        sut.Update(false);

        sut.MarkUnavailable();

        Assert.False(sut.IsRedisHealthy);
    }
}
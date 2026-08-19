using Core.Cache.Rehydration.Abstractions;

namespace Core.Cache.Rehydration.UnitTests.Abstractions;

public sealed class CacheRehydrationEntryTests
{
    [Fact]
    public void CanCreateEntry_WithRequiredProperties()
    {
        var entry = new CacheRehydrationEntry
        {
            Key = "cache:key",
            Value = "value"
        };

        Assert.Equal(
            "cache:key",
            entry.Key);

        Assert.Equal(
            "value",
            entry.Value);
    }

    [Fact]
    public void CanCreateEntry_WithExpirationAndTags()
    {
        var expiration = TimeSpan.FromMinutes(5);
        var tags = new[] { "users", "active" };

        var entry = new CacheRehydrationEntry
        {
            Key = "cache:key",
            Value = "value",
            RemainingExpiration = expiration,
            Tags = tags
        };

        Assert.Equal(
            expiration,
            entry.RemainingExpiration);

        Assert.Equal(
            tags,
            entry.Tags);
    }

    [Fact]
    public void OptionalProperties_DefaultToNull()
    {
        var entry = new CacheRehydrationEntry
        {
            Key = "cache:key",
            Value = "value"
        };

        Assert.Null(
            entry.RemainingExpiration);

        Assert.Null(
            entry.Tags);
    }
}
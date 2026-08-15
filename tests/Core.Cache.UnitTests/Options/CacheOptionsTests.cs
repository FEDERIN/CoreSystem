using Core.Cache.Options;
using Core.Serialization;

namespace Core.Cache.UnitTests.Options;

public sealed class CacheOptionsTests
{
    [Fact]
    public void CopyFrom_ShouldCopyValuesFromSource()
    {
        var source = new CacheOptions
        {
            Enabled = false,
            InstanceName = "my-app",
            DefaultExpiration = TimeSpan.FromMinutes(10),
            MaxCacheableSize = 1024,
            SerializerType = SerializerType.Json
        };

        var target = new CacheOptions();

        target.CopyFrom(source);

        Assert.Equal(source.Enabled, target.Enabled);
        Assert.Equal(source.InstanceName, target.InstanceName);
        Assert.Equal(source.DefaultExpiration, target.DefaultExpiration);
        Assert.Equal(source.MaxCacheableSize, target.MaxCacheableSize);
        Assert.Equal(source.SerializerType, target.SerializerType);
    }
}

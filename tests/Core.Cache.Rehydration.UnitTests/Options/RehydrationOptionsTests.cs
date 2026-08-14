using Core.Cache.Rehydration.Options;

namespace Core.Cache.Rehydration.UnitTests.Options;

public sealed class RehydrationOptionsTests
{
    [Fact]
    public void CopyFrom_CopiesValuesFromSource()
    {
        var source = new RehydrationOptions
        {
            Enabled = false,
            Interval = TimeSpan.FromMinutes(5)
        };

        var target = new RehydrationOptions
        {
            Enabled = true,
            Interval = TimeSpan.FromSeconds(30)
        };

        target.CopyFrom(source);

        Assert.False(target.Enabled);
        Assert.Equal(
            TimeSpan.FromMinutes(5),
            target.Interval);
    }

    [Fact]
    public void CopyFrom_ThrowsArgumentNullException_WhenSourceIsNull()
    {
        var options = new RehydrationOptions();

        Assert.Throws<ArgumentNullException>(
            () => options.CopyFrom(null!));
    }
}
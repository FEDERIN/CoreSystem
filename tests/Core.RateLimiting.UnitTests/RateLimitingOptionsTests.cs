using Core.RateLimiting.Options;
using FluentAssertions;
using System.Threading.RateLimiting;

namespace Core.RateLimiting.UnitTests;

public sealed class RateLimitingOptionsTests
{
    [Fact]
    public void CopyFrom_CopiesEveryPublicOptionAndReturnsCurrentInstance()
    {
        var source = new RateLimitingOptions
        {
            Enabled = false,
            PolicyName = "management-api",
            PermitLimit = 60,
            Window = TimeSpan.FromSeconds(30),
            QueueLimit = 5,
            QueueProcessingOrder = QueueProcessingOrder.NewestFirst,
            AutoReplenishment = false,
            SubjectClaimType = "sub"
        };
        var target = new RateLimitingOptions();

        var result = target.CopyFrom(source);

        result.Should().BeSameAs(target);
        target.Enabled.Should().BeFalse();
        target.PolicyName.Should().Be("management-api");
        target.PermitLimit.Should().Be(60);
        target.Window.Should().Be(TimeSpan.FromSeconds(30));
        target.QueueLimit.Should().Be(5);
        target.QueueProcessingOrder.Should().Be(QueueProcessingOrder.NewestFirst);
        target.AutoReplenishment.Should().BeFalse();
        target.SubjectClaimType.Should().Be("sub");
    }

    [Fact]
    public void CopyFrom_Throws_WhenSourceIsNull()
    {
        var target = new RateLimitingOptions();

        var action = () => target.CopyFrom(null!);

        action.Should().Throw<ArgumentNullException>()
            .WithParameterName("source");
    }

    [Fact]
    public void AddCoreRateLimiting_RejectsInvalidPermitLimit()
    {
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        var action = () => DependencyInjection.RateLimitingRegistration.AddCoreRateLimiting(services, options => options.PermitLimit = 0);

        action.Should()
            .Throw<ArgumentOutOfRangeException>()
            .WithParameterName("PermitLimit")
            .WithMessage("Rate-limit permit limit must be greater than zero.*");
    }
}

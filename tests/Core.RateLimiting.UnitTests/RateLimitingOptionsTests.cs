using FluentAssertions;

namespace Core.RateLimiting.UnitTests;

public sealed class RateLimitingOptionsTests
{
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

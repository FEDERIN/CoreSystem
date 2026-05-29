using Core.Idempotency.Options;
using FluentAssertions;
using Xunit;

namespace Core.Idempotency.Tests.Options;

public class IdempotencyOptionsTests
{
    [Fact]
    public void DefaultOptions_HasExpectedValues()
    {
        // Arrange & Act
        var options = new IdempotencyOptions();

        // Assert
        options.Enabled.Should().BeTrue();
        options.HeaderName.Should().Be("X-Idempotency-Key");
        options.Expiration.Should().Be(TimeSpan.FromHours(24));
        options.AllowedMethods.Should().Contain("POST", "PUT");
        options.Provider.Should().Be("Redis");
        options.MeterName.Should().BeNull();
    }

    [Fact]
    public void CanSetCustomValues()
    {
        // Arrange
        var options = new IdempotencyOptions
        {
            Enabled = false,
            HeaderName = "X-Custom-Key",
            Expiration = TimeSpan.FromHours(6),
            AllowedMethods = ["DELETE"],
            Provider = "PostgreSQL",
            MeterName = "custom.meter"
        };

        // Assert
        options.Enabled.Should().BeFalse();
        options.HeaderName.Should().Be("X-Custom-Key");
        options.Expiration.Should().Be(TimeSpan.FromHours(6));
        options.AllowedMethods.Should().Contain("DELETE");
        options.Provider.Should().Be("PostgreSQL");
        options.MeterName.Should().Be("custom.meter");
    }

    [Fact]
    public void AllowedMethodsCanBeModified()
    {
        // Arrange
        var options = new IdempotencyOptions();

        // Act
        options.AllowedMethods = ["POST", "PUT", "PATCH", "DELETE"];

        // Assert
        options.AllowedMethods.Should().HaveCount(4);
        options.AllowedMethods.Should().Contain("PATCH");
    }
}
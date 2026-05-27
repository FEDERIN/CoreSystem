using Core.Idempotency.Models;
using FluentAssertions;
using Xunit;

namespace Core.Idempotency.Tests.Models;

public class IdempotencyResponseTests
{
    [Fact]
    public void Constructor_CreatesInstanceWithDefaultValues()
    {
        // Arrange & Act
        var response = new IdempotencyResponse();

        // Assert
        response.StatusCode.Should().Be(0);
        response.ContentType.Should().BeNull();
        response.Body.Should().BeNull();
        response.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void CanSetProperties()
    {
        // Arrange
        var response = new IdempotencyResponse();
        var responseBody = "{\"id\":1,\"name\":\"Test\"}";
        var createdAt = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        // Act
        response.StatusCode = 200;
        response.ContentType = "application/json";
        response.Body = responseBody;
        response.CreatedAt = createdAt;

        // Assert
        response.StatusCode.Should().Be(200);
        response.ContentType.Should().Be("application/json");
        response.Body.Should().Be(responseBody);
        response.CreatedAt.Should().Be(createdAt);
    }

    [Fact]
    public void SerializesAndDeserializesCorrectly()
    {
        // Arrange
        var original = new IdempotencyResponse
        {
            StatusCode = 201,
            ContentType = "application/json",
            Body = "{\"success\":true}"
        };

        // Act
        var json = System.Text.Json.JsonSerializer.Serialize(original);
        var deserialized = System.Text.Json.JsonSerializer.Deserialize<IdempotencyResponse>(json)!;

        // Assert
        deserialized.StatusCode.Should().Be(original.StatusCode);
        deserialized.ContentType.Should().Be(original.ContentType);
        deserialized.Body.Should().Be(original.Body);
    }
}
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Core.Serialization.Json;

namespace Core.Serialization.UnitTests.Json;

public class JsonPayloadSerializerTests
{
    private readonly JsonPayloadSerializer _serializer;

    public JsonPayloadSerializerTests()
    {
        _serializer = new JsonPayloadSerializer(new JsonSerializerOptions());
    }

    [Fact]
    public void Serialize_ShouldSerializeObject()
    {
        // Arrange
        var model = new TestModel
        {
            Id = 1,
            Name = "Test"
        };

        // Act
        var result = _serializer.Serialize(model);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();

        var json = Encoding.UTF8.GetString(result);

        json.Should().Contain("Id");
        json.Should().Contain("Name");
    }

    [Fact]
    public void Serialize_ShouldSerializePrimitive()
    {
        // Act
        var result = _serializer.Serialize(123);

        // Assert
        Encoding.UTF8.GetString(result)
            .Should()
            .Be("123");
    }

    [Fact]
    public void Serialize_ShouldRespectConfiguredOptions()
    {
        // Arrange
        var serializer = new JsonPayloadSerializer(
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

        var model = new TestModel
        {
            Id = 5,
            Name = "John"
        };

        // Act
        var bytes = serializer.Serialize(model);

        // Assert
        var json = Encoding.UTF8.GetString(bytes);

        json.Should().Contain("id");
        json.Should().Contain("name");
        json.Should().NotContain("\"Id\"");
        json.Should().NotContain("\"Name\"");
    }

    [Fact]
    public void Serialize_ShouldThrowCoreSerializationException_WhenSerializationFails()
    {
        // Arrange
        var node = new CircularNode();
        node.Next = node;

        // Act
        var act = () => _serializer.Serialize(node);

        // Assert
        var exception = act.Should()
            .Throw<CoreSerializationException>()
            .Which;

        exception.Serializer.Should().Be(SerializerType.Json);
        exception.InnerException.Should().NotBeNull();
    }

    [Fact]
    public void Deserialize_ShouldDeserializeObject()
    {
        // Arrange
        var model = new TestModel
        {
            Id = 10,
            Name = "John"
        };

        var bytes = _serializer.Serialize(model);

        // Act
        var result = _serializer.Deserialize<TestModel>(bytes);

        // Assert
        result.Should().BeEquivalentTo(model);
    }

    [Fact]
    public void Deserialize_ShouldDeserializeCollection()
    {
        // Arrange
        var models = new[]
        {
            new TestModel { Id = 1, Name = "One" },
            new TestModel { Id = 2, Name = "Two" }
        };

        var bytes = _serializer.Serialize(models);

        // Act
        var result = _serializer.Deserialize<TestModel[]>(bytes);

        // Assert
        result.Should().BeEquivalentTo(models);
    }

    [Fact]
    public void Deserialize_ShouldReturnDefault_WhenPayloadIsEmpty()
    {
        // Act
        var result = _serializer.Deserialize<TestModel>([]);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Deserialize_ShouldThrowCoreSerializationException_WhenPayloadIsInvalid()
    {
        // Arrange
        var invalid = Encoding.UTF8.GetBytes("{ invalid json");

        // Act
        var act = () => _serializer.Deserialize<TestModel>(invalid);

        // Assert
        var exception = act.Should()
            .Throw<CoreSerializationException>()
            .Which;

        exception.Serializer.Should().Be(SerializerType.Json);
        exception.InnerException.Should().NotBeNull();
    }

    private sealed class TestModel
    {
        public int Id { get; set; }

        public string? Name { get; set; }
    }

    private sealed class CircularNode
    {
        public CircularNode? Next { get; set; }
    }
}
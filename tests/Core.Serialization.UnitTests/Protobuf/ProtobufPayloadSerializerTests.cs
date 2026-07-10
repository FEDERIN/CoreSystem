using Core.Serialization.Protobuf;
using FluentAssertions;
using ProtoBuf;


namespace Core.Serialization.UnitTests.Protobuf;

public sealed class ProtobufPayloadSerializerTests
{
    private readonly ProtobufPayloadSerializer _serializer = new();

    [Fact]
    public void Serialize_ShouldReturnBytes()
    {
        // Arrange
        var model = new TestModel
        {
            Id = 10,
            Name = "Jhon Doe"
        };

        // Act
        var bytes = _serializer.Serialize(model);

        // Assert
        bytes.Should().NotBeNull();
        bytes.Should().NotBeEmpty();
    }

    [Fact]
    public void Deserialize_ShouldReturnObject()
    {
        // Arrange
        var model = new TestModel
        {
            Id = 5,
            Name = "Core"
        };

        var bytes = _serializer.Serialize(model);

        // Act
        var result = _serializer.Deserialize<TestModel>(bytes);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(5);
        result.Name.Should().Be("Core");
    }

    [Fact]
    public void Deserialize_WhenPayloadIsEmpty_ShouldReturnDefault()
    {
        // Arrange
        var bytes = Array.Empty<byte>();

        // Act
        var result = _serializer.Deserialize<TestModel>(bytes);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void Deserialize_WhenPayloadIsInvalid_ShouldThrowCoreSerializationException()
    {
        // Arrange
        var bytes = new byte[] { 1, 2, 3, 4, 5 };

        // Act
        var action = () => _serializer.Deserialize<TestModel>(bytes);

        // Assert
        var exception = action.Should()
            .Throw<CoreSerializationException>()
            .Which;

        exception.Serializer.Should().Be(SerializerType.Protobuf);
        exception.InnerException.Should().NotBeNull();
    }

    [Fact]
    public void Serialize_WhenObjectCannotBeSerialized_ShouldThrowCoreSerializationException()
    {
        // Arrange
        var model = new InvalidModel();

        // Act
        var action = () => _serializer.Serialize(model);

        // Assert
        var exception = action.Should()
            .Throw<CoreSerializationException>()
            .Which;

        exception.Serializer.Should().Be(SerializerType.Protobuf);
        exception.InnerException.Should().NotBeNull();
    }
}

[ProtoContract]
internal sealed class TestModel
{
    [ProtoMember(1)]
    public int Id { get; set; }

    [ProtoMember(2)]
    public string Name { get; set; } = string.Empty;
}

internal sealed class InvalidModel
{
    public Stream Stream { get; set; } = Stream.Null;
}
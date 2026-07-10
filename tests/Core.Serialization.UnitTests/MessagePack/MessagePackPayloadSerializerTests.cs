using Core.Serialization.MessagePack;
using FluentAssertions;
using MessagePack;
using MessagePack.Resolvers;

namespace Core.Serialization.UnitTests.MessagePack;

public sealed class MessagePackPayloadSerializerTests
{
    private readonly MessagePackPayloadSerializer _serializer =
        new(
            MessagePackSerializerOptions.Standard
                .WithResolver(ContractlessStandardResolver.Instance));

    private sealed class Person
    {
        public string Name { get; init; } = default!;
        public int Age { get; init; }
    }

    [Fact]
    public void Serialize_Primitive_ShouldRoundTrip()
    {
        // Arrange
        const int value = 12345;

        // Act
        var bytes = _serializer.Serialize(value);
        var result = _serializer.Deserialize<int>(bytes);

        // Assert
        result.Should().Be(value);
    }

    [Fact]
    public void Serialize_List_ShouldRoundTrip()
    {
        // Arrange
        var list = new List<string>
        {
            "A",
            "B",
            "C"
        };

        // Act
        var bytes = _serializer.Serialize(list);
        var result = _serializer.Deserialize<List<string>>(bytes);

        // Assert
        result.Should().Equal(list);
    }

    [Fact]
    public void Deserialize_InvalidPayload_ShouldThrowCoreSerializationException()
    {
        // Arrange
        var invalid = new byte[] { 255, 254, 253, 252 };

        // Act
        var action = () => _serializer.Deserialize<Person>(invalid);

        // Assert
        var exception = action.Should()
            .Throw<CoreSerializationException>()
            .Which;

        exception.Serializer.Should().Be(SerializerType.MessagePack);
        exception.InnerException.Should().NotBeNull();
    }

    [Fact]
    public void Serialize_WhenObjectContainsDelegate_ShouldThrowCoreSerializationException()
    {
        // Arrange
        var value = new NotSerializable
        {
            Callback = () => { }
        };

        // Act
        var act = () => _serializer.Serialize(value);

        // Assert
        var exception = act.Should()
            .Throw<CoreSerializationException>()
            .Which;

        exception.Serializer.Should().Be(SerializerType.MessagePack);
        exception.InnerException.Should().NotBeNull();
    }

    [Fact]
    public void Deserialize_WhenBytesAreEmpty_ShouldReturnDefault()
    {
        // Arrange
        var serializer = new MessagePackPayloadSerializer(
            MessagePackSerializerOptions.Standard
                .WithResolver(ContractlessStandardResolver.Instance));

        // Act
        var result = serializer.Deserialize<Person>(Array.Empty<byte>());

        // Assert
        result.Should().BeNull();
    }

    private sealed class NotSerializable
    {
        public Action? Callback { get; set; }
    }
}
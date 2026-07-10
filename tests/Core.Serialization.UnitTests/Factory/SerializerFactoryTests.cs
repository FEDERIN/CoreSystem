using Core.Serialization.Abstractions;
using Core.Serialization.Json;
using Core.Serialization.MessagePack;
using Core.Serialization.Protobuf;
using FluentAssertions;
using MessagePack;
using Org.BouncyCastle.Ocsp;
using Xunit;

namespace Core.Serialization.UnitTests;

public class SerializerFactoryTests
{
    private readonly JsonPayloadSerializer _json =
        new(new System.Text.Json.JsonSerializerOptions());

    private readonly MessagePackPayloadSerializer _messagePack =
        new(MessagePackSerializerOptions.Standard);

    private readonly ProtobufPayloadSerializer _protobuf =
        new();

    private readonly SerializerFactory _factory;

    public SerializerFactoryTests()
    {
        _factory = new SerializerFactory(
            _json,
            _messagePack,
            _protobuf);
    }

    [Fact]
    public void GetSerializer_WhenJson_ShouldReturnJsonSerializer()
    {
        // Act
        var result = _factory.GetSerializer(SerializerType.Json);

        // Assert
        result.Should().BeSameAs(_json);
    }

    [Fact]
    public void GetSerializer_WhenMessagePack_ShouldReturnMessagePackSerializer()
    {
        // Act
        var result = _factory.GetSerializer(SerializerType.MessagePack);

        // Assert
        result.Should().BeSameAs(_messagePack);
    }

    [Fact]
    public void GetSerializer_WhenProtobuf_ShouldReturnProtobufSerializer()
    {
        // Act
        var result = _factory.GetSerializer(SerializerType.Protobuf);

        // Assert
        result.Should().BeSameAs(_protobuf);
    }

    [Fact]
    public void GetSerializer_WhenSerializerIsNotSupported_ShouldThrow()
    {
        // Act
        var act = () => _factory.GetSerializer(unchecked((SerializerType)255));

        // Assert
        act.Should()
            .Throw<NotSupportedException>()
            .WithMessage("Serializer '255' is not supported.");
    }
}
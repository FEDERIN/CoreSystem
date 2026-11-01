using Core.OpenApi.Attributes;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace Core.OpenApi.UnitTests;

public sealed class ProducesResponseAttributesTests
{
    [Fact]
    public void ProducesOk_Declares200()
    {
        var attribute = new ProducesOkAttribute<string>();

        attribute.StatusCode.Should().Be(StatusCodes.Status200OK);
        attribute.Type.Should().Be<string>();
    }

    [Fact]
    public void ProducesCreated_Declares201()
    {
        var attribute = new ProducesCreatedAttribute<string>();

        attribute.StatusCode.Should().Be(StatusCodes.Status201Created);
        attribute.Type.Should().Be<string>();
    }

    [Fact]
    public void ProducesNoContent_Declares204WithoutBody()
    {
        var attribute = new ProducesNoContentAttribute();

        attribute.StatusCode.Should().Be(StatusCodes.Status204NoContent);
        Assert.Equal(typeof(void), attribute.Type);
    }
}

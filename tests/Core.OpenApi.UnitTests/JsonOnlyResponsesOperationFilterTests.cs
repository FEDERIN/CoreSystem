using Core.OpenApi.Filters;
using FluentAssertions;
using Microsoft.OpenApi.Models;

namespace Core.OpenApi.UnitTests;

public sealed class JsonOnlyResponsesOperationFilterTests
{
    [Fact]
    public void Apply_ShouldRemoveNonJsonContent_WhenJsonResponseExists()
    {
        var operation = new OpenApiOperation
        {
            Responses = new OpenApiResponses
            {
                ["200"] = new OpenApiResponse
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new OpenApiMediaType(),
                        ["text/plain"] = new OpenApiMediaType(),
                        ["text/json"] = new OpenApiMediaType()
                    }
                }
            }
        };

        new JsonOnlyResponsesOperationFilter().Apply(operation, null!);

        var content = operation.Responses["200"].Content;

        content.Should().ContainKey("application/json");
        content.Should().NotContainKey("text/plain");
        content.Should().NotContainKey("text/json");
        content.Should().HaveCount(1);
    }

    [Fact]
    public void Apply_ShouldKeepOtherContent_WhenJsonIsMissing()
    {
        var operation = new OpenApiOperation
        {
            Responses = new OpenApiResponses
            {
                ["200"] = new OpenApiResponse
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["text/plain"] = new OpenApiMediaType()
                    }
                }
            }
        };

        new JsonOnlyResponsesOperationFilter().Apply(operation, null!);

        operation.Responses["200"].Content.Should().ContainKey("text/plain");
        operation.Responses["200"].Content.Should().HaveCount(1);
    }
}

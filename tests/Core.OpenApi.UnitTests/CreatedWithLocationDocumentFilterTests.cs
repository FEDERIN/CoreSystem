using Core.OpenApi.Filters;
using FluentAssertions;
using Microsoft.OpenApi.Models;

namespace Core.OpenApi.UnitTests;

public sealed class CreatedWithLocationDocumentFilterTests
{
    [Fact]
    public void Apply_AddsLocationHeader_ToOperationsWith201Response()
    {
        var document = CreateDocument(
            OperationType.Post,
            new OpenApiResponse
            {
                Description = "Created",
                Headers = null
            },
            "201");

        new CreatedWithLocationDocumentFilter().Apply(document, null!);

        var response = document.Paths["/artists"].Operations[OperationType.Post].Responses["201"];

        response.Headers.Should().NotBeNull();
        response.Headers.Should().ContainKey("Location");

        var location = response.Headers!["Location"];

        location.Description.Should().Be("URL of the newly created resource");
        location.Schema.Type.Should().Be("string");
        location.Schema.Format.Should().Be("uri");
    }

    [Fact]
    public void Apply_DoesNotAddLocationHeader_WhenNo201Response()
    {
        var document = CreateDocument(
            OperationType.Get,
            new OpenApiResponse { Description = "OK" },
            "200");

        new CreatedWithLocationDocumentFilter().Apply(document, null!);

        var response = document.Paths["/artists"].Operations[OperationType.Get].Responses["200"];

        response.Headers.Should().NotContainKey("Location");
    }

    [Fact]
    public void Apply_IsIdempotent_DoesNotOverrideExistingHeader()
    {
        var document = CreateDocument(
            OperationType.Post,
            new OpenApiResponse
            {
                Description = "Created",
                Headers = new Dictionary<string, OpenApiHeader>
                {
                    ["Location"] = new OpenApiHeader
                    {
                        Description = "existing",
                        Schema = new OpenApiSchema { Type = "string" }
                    }
                }
            },
            "201");

        var filter = new CreatedWithLocationDocumentFilter();
        filter.Apply(document, null!);
        filter.Apply(document, null!);

        var response = document.Paths["/artists"].Operations[OperationType.Post].Responses["201"];

        response.Headers.Should().ContainSingle();
        response.Headers!["Location"].Description.Should().Be("existing");
    }

    private static OpenApiDocument CreateDocument(
        OperationType method,
        OpenApiResponse response,
        string statusCode) =>
        new()
        {
            Paths = new OpenApiPaths
            {
                ["/artists"] = new OpenApiPathItem
                {
                    Operations =
                    {
                        [method] = new OpenApiOperation
                        {
                            Responses = new OpenApiResponses
                            {
                                [statusCode] = response
                            }
                        }
                    }
                }
            }
        };
}

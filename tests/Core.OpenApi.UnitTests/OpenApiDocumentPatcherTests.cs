using System.Text.Json.Nodes;
using Core.OpenApi.Options;
using FluentAssertions;

namespace Core.OpenApi.UnitTests;

public sealed class OpenApiDocumentPatcherTests
{
    [Fact]
    public void Patch_WhenJsonIsInvalid_ReturnsOriginal()
    {
        var input = "{ not-json }";

        var output = Patch(input);

        output.Should().Be(input);
    }

    [Fact]
    public void Patch_WhenJsonHasNoPaths_StillAddsCommonResponses()
    {
        var input = """
        {
          "openapi": "3.0.4",
          "info": { "title": "X", "version": "v1" }
        }
        """;

        var responses = ComponentsResponses(Patch(input));

        responses.ContainsKey("Unauthorized").Should().BeTrue();
        responses.ContainsKey("Forbidden").Should().BeTrue();
        responses.ContainsKey("InternalError").Should().BeTrue();
        responses.ContainsKey("NotFound").Should().BeTrue();
        responses.ContainsKey("Conflict").Should().BeTrue();
        responses.ContainsKey("ValidationError").Should().BeTrue();
        responses.ContainsKey("TooManyRequests").Should().BeTrue();
    }

    [Fact]
    public void Patch_ReplacesKnownStatusCodesWithRefs_WhenTheyExist()
    {
        var input = """
        {
          "openapi": "3.0.4",
          "info": { "title": "X", "version": "v1" },
          "paths": {
            "/api/v1/artists/{id}": {
              "get": {
                "responses": {
                  "200": { "description": "OK" },
                  "401": { "description": "whatever" },
                  "403": { "description": "whatever" },
                  "404": { "description": "whatever" },
                  "409": { "description": "whatever" },
                  "429": { "description": "whatever" },
                  "500": { "description": "whatever" }
                }
              }
            }
          }
        }
        """;

        var responses = OperationResponses(Patch(input), "/api/v1/artists/{id}", "get");

        Reference(responses, "401").Should().Be("#/components/responses/Unauthorized");
        Reference(responses, "403").Should().Be("#/components/responses/Forbidden");
        Reference(responses, "404").Should().Be("#/components/responses/NotFound");
        Reference(responses, "409").Should().Be("#/components/responses/Conflict");
        Reference(responses, "429").Should().Be("#/components/responses/TooManyRequests");
        Reference(responses, "500").Should().Be("#/components/responses/InternalError");

        responses["200"]!.AsObject().ContainsKey("$ref").Should().BeFalse();
    }

    [Fact]
    public void Patch_DoesNotAddRefForStatusCodeThatDoesNotExist()
    {
        var input = """
        {
          "openapi": "3.0.4",
          "info": { "title": "X", "version": "v1" },
          "paths": {
            "/x": {
              "get": {
                "responses": {
                  "200": { "description": "OK" }
                }
              }
            }
          }
        }
        """;

        var responses = OperationResponses(Patch(input), "/x", "get");

        responses.ContainsKey("200").Should().BeTrue();
        responses.ContainsKey("401").Should().BeFalse();
    }

    [Fact]
    public void Patch_AddsResponse_WhenDefinitionIncludesMissingOperations()
    {
        var options = new OpenApiProblemResponsesOptions();
        options.Definitions
            .First(definition => definition.StatusCode == "429")
            .IncludeWhenMissing = true;

        var input = """
        {
          "openapi": "3.0.4",
          "info": { "title": "X", "version": "v1" },
          "paths": {
            "/x": {
              "get": {
                "responses": {
                  "200": { "description": "OK" }
                }
              }
            }
          }
        }
        """;

        var responses = OperationResponses(
            OpenApiDocumentPatcher.Patch(input, options),
            "/x",
            "get");

        Reference(responses, "429").Should().Be("#/components/responses/TooManyRequests");
    }

    [Fact]
    public void Patch_Replaces400WithValidationError_WhenContentIsProblemJson()
    {
        var input = """
        {
          "openapi": "3.0.4",
          "info": { "title": "X", "version": "v1" },
          "paths": {
            "/x": {
              "post": {
                "responses": {
                  "400": {
                    "description": "Bad Request",
                    "content": {
                      "application/problem+json": {
                        "schema": { "$ref": "#/components/schemas/ValidationProblemDetails" }
                      }
                    }
                  }
                }
              }
            }
          }
        }
        """;

        var responses = OperationResponses(Patch(input), "/x", "post");

        Reference(responses, "400").Should().Be("#/components/responses/ValidationError");
    }

    [Fact]
    public void Patch_Replaces400WithValidationError_WhenSchemaIsValidationProblemDetails()
    {
        var input = """
        {
          "openapi": "3.0.4",
          "info": { "title": "X", "version": "v1" },
          "paths": {
            "/x": {
              "post": {
                "responses": {
                  "400": {
                    "description": "Bad Request",
                    "content": {
                      "application/json": {
                        "schema": { "$ref": "#/components/schemas/ValidationProblemDetails" }
                      }
                    }
                  }
                }
              }
            }
          }
        }
        """;

        var responses = OperationResponses(Patch(input), "/x", "post");

        Reference(responses, "400").Should().Be("#/components/responses/ValidationError");
    }

    [Fact]
    public void Patch_Replaces400_WhenContentIsMissing()
    {
        var input = """
        {
          "openapi": "3.0.4",
          "info": { "title": "X", "version": "v1" },
          "paths": {
            "/x": {
              "post": {
                "responses": {
                  "400": { "description": "Bad Request" }
                }
              }
            }
          }
        }
        """;

        var responses = OperationResponses(Patch(input), "/x", "post");

        Reference(responses, "400").Should().Be("#/components/responses/ValidationError");
    }

    [Fact]
    public void Patch_KeepsCustom400_WhenDefinitionOnlyMatchesProblemContent()
    {
        var input = """
        {
          "openapi": "3.0.4",
          "info": { "title": "X", "version": "v1" },
          "paths": {
            "/x": {
              "post": {
                "responses": {
                  "400": {
                    "description": "Bad Request",
                    "content": {
                      "application/json": {
                        "schema": { "$ref": "#/components/schemas/CustomError" }
                      }
                    }
                  }
                }
              }
            }
          }
        }
        """;

        var responses = OperationResponses(Patch(input), "/x", "post");

        responses["400"]!.AsObject().ContainsKey("$ref").Should().BeFalse();
    }

    [Fact]
    public void Patch_DoesNotTouch400_WhenItIsAlreadyARef()
    {
        var input = """
        {
          "openapi": "3.0.4",
          "info": { "title": "X", "version": "v1" },
          "paths": {
            "/x": {
              "post": {
                "responses": {
                  "400": { "$ref": "#/components/responses/ValidationError" }
                }
              }
            }
          }
        }
        """;

        var responses = OperationResponses(Patch(input), "/x", "post");

        Reference(responses, "400").Should().Be("#/components/responses/ValidationError");
    }

    [Fact]
    public void Patch_IsIdempotent_RunningTwiceProducesSameJsonStructure()
    {
        var input = """
        {
          "openapi": "3.0.4",
          "info": { "title": "X", "version": "v1" },
          "paths": {
            "/x": {
              "get": { "responses": { "401": { "description": "x" }, "500": { "description": "x" } } }
            }
          }
        }
        """;

        var once = Patch(input);
        var twice = Patch(once);

        JsonNode.DeepEquals(JsonNode.Parse(once), JsonNode.Parse(twice)).Should().BeTrue();
    }

    [Fact]
    public void Patch_TooManyRequestsResponse_HasRetryAfterHeader()
    {
        var input = """
        {
          "openapi": "3.0.4",
          "info": { "title": "X", "version": "v1" }
        }
        """;

        var tooManyRequests = ComponentsResponses(Patch(input))["TooManyRequests"]!.AsObject();
        var headers = tooManyRequests["headers"]!.AsObject();

        headers.ContainsKey("Retry-After").Should().BeTrue();
        headers["Retry-After"]!["schema"]!["type"]!.GetValue<string>().Should().Be("integer");
    }

    [Fact]
    public void Patch_UsesConfiguredSchemaAndTypePrefix()
    {
        var options = new OpenApiProblemResponsesOptions
        {
            SchemaId = "AppProblemDetails",
            TypePrefix = "urn:app:error:",
            TraceIdExample = "00-aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa-bbbbbbbbbbbbbbbb-01"
        };

        var input = """
        {
          "openapi": "3.0.4",
          "info": { "title": "X", "version": "v1" }
        }
        """;

        var content = ComponentsResponses(OpenApiDocumentPatcher.Patch(input, options))["Unauthorized"]!
            .AsObject()["content"]!
            .AsObject();

        content["application/problem+json"]!["schema"]!["$ref"]!
            .GetValue<string>()
            .Should()
            .Be("#/components/schemas/AppProblemDetails");

        var example = content["application/problem+json"]!["example"]!;

        example["type"]!.GetValue<string>().Should().Be("urn:app:error:UNAUTHORIZED");
        example["errorCode"]!.GetValue<string>().Should().Be("UNAUTHORIZED");
        example["traceId"]!.GetValue<string>()
            .Should()
            .Be("00-aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa-bbbbbbbbbbbbbbbb-01");
    }

    [Fact]
    public void Patch_KeepsExistingComponent_WhenGeneratorAlreadyDeclaredIt()
    {
        var input = """
        {
          "openapi": "3.0.4",
          "info": { "title": "X", "version": "v1" },
          "components": {
            "responses": {
              "NotFound": { "description": "Custom" }
            }
          }
        }
        """;

        var notFound = ComponentsResponses(Patch(input))["NotFound"]!.AsObject();

        notFound["description"]!.GetValue<string>().Should().Be("Custom");
    }

    [Fact]
    public void Patch_WhenJsonIsValidButNotObject_ReturnsOriginal()
    {
        var input = "[]";

        Patch(input).Should().Be(input);
    }

    private static string Patch(string json) =>
        OpenApiDocumentPatcher.Patch(json, new OpenApiProblemResponsesOptions());

    private static JsonObject ComponentsResponses(string json) =>
        JsonNode.Parse(json)!["components"]!["responses"]!.AsObject();

    private static JsonObject OperationResponses(
        string json,
        string path,
        string method) =>
        JsonNode.Parse(json)!["paths"]![path]![method]!["responses"]!.AsObject();

    private static string? Reference(JsonObject responses, string statusCode) =>
        responses[statusCode]?["$ref"]?.GetValue<string>();
}

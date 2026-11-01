using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using Core.OpenApi.Options;

namespace Core.OpenApi;

/// <summary>
/// Rewrites an OpenAPI document so that repeated error responses are declared once as
/// references instead of being repeated inline on every operation.
/// </summary>
/// <remarks>
/// The patcher registers the configured responses in <c>components.responses</c> and then
/// replaces, or optionally adds, the matching status codes on every operation with a
/// <c>$ref</c> to the registered component. Invalid JSON and documents that are not JSON
/// objects are returned unchanged, which makes the patcher safe to apply to the output of
/// any generator.
/// </remarks>
public static class OpenApiDocumentPatcher
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };

    /// <summary>
    /// Applies the problem-response catalog to an OpenAPI document.
    /// </summary>
    /// <param name="document">
    /// The serialized OpenAPI document to patch.
    /// </param>
    /// <param name="options">
    /// The response catalog to apply.
    /// </param>
    /// <returns>
    /// The patched document, or the original value when the document cannot be parsed.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="document"/> or <paramref name="options"/> is
    /// <see langword="null"/>.
    /// </exception>
    public static string Patch(string document, OpenApiProblemResponsesOptions options)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(options);

        if (!options.Enabled)
        {
            return document;
        }

        JsonNode? parsed;
        try
        {
            parsed = JsonNode.Parse(document);
        }
        catch (JsonException)
        {
            return document;
        }

        if (parsed is not JsonObject root)
        {
            return document;
        }

        if (options.Definitions.Count > 0)
        {
            EnsureCommonResponses(root, options);
        }

        ReplaceResponsesWithRefs(root, options);

        return root.ToJsonString(SerializerOptions);
    }

    /// <summary>
    /// Creates the <c>components.responses</c> entries that are missing.
    /// </summary>
    /// <param name="root">The document root.</param>
    /// <param name="options">The response catalog.</param>
    private static void EnsureCommonResponses(
        JsonObject root,
        OpenApiProblemResponsesOptions options)
    {
        var components = root["components"] as JsonObject ?? [];
        root["components"] = components;

        var responses = components["responses"] as JsonObject ?? [];
        components["responses"] = responses;

        foreach (var definition in options.Definitions)
        {
            responses.TryAdd(definition.ComponentName, BuildResponse(definition, options));
        }
    }

    /// <summary>
    /// Builds the component for one response definition.
    /// </summary>
    /// <param name="definition">The response definition.</param>
    /// <param name="options">The response catalog.</param>
    /// <returns>The response component.</returns>
    private static JsonObject BuildResponse(
        ProblemResponseDefinition definition,
        OpenApiProblemResponsesOptions options)
    {
        var response = new JsonObject
        {
            ["description"] = definition.Description ?? definition.Title ?? definition.ErrorCode
        };

        if (definition.AddRetryAfterHeader)
        {
            response["headers"] = new JsonObject
            {
                ["Retry-After"] = new JsonObject
                {
                    ["description"] = "Seconds to wait before retrying the request.",
                    ["schema"] = new JsonObject
                    {
                        ["type"] = "integer",
                        ["format"] = "int32"
                    }
                }
            };
        }

        var schemaId = definition.SchemaId ?? options.SchemaId;

        response["content"] = new JsonObject
        {
            [options.ContentType] = new JsonObject
            {
                ["schema"] = new JsonObject
                {
                    ["$ref"] = $"#/components/schemas/{schemaId}"
                },
                ["example"] = BuildExample(definition, options)
            }
        };

        return response;
    }

    /// <summary>
    /// Builds the example payload of one response definition.
    /// </summary>
    /// <param name="definition">The response definition.</param>
    /// <param name="options">The response catalog.</param>
    /// <returns>The example payload.</returns>
    private static JsonObject BuildExample(
        ProblemResponseDefinition definition,
        OpenApiProblemResponsesOptions options)
    {
        var example = new JsonObject();

        if (!string.IsNullOrEmpty(options.TypePrefix))
        {
            example["type"] = string.Concat(options.TypePrefix, definition.ErrorCode);
        }

        if (definition.Title is not null)
        {
            example["title"] = definition.Title;
        }

        if (int.TryParse(
            definition.StatusCode,
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out var status))
        {
            example["status"] = status;
        }

        if (definition.Detail is not null)
        {
            example["detail"] = definition.Detail;
        }

        example["errorCode"] = definition.ErrorCode;

        foreach (var field in definition.AdditionalExampleFields)
        {
            example[field.Key] = ToJsonNode(field.Value);
        }

        if (options.TraceIdExample is not null)
        {
            example["traceId"] = options.TraceIdExample;
        }

        return example;
    }

    /// <summary>
    /// Replaces the responses of every operation with references to the components.
    /// </summary>
    /// <param name="root">The document root.</param>
    /// <param name="options">The response catalog.</param>
    private static void ReplaceResponsesWithRefs(
        JsonObject root,
        OpenApiProblemResponsesOptions options)
    {
        if (root["paths"] is not JsonObject paths)
        {
            return;
        }

        foreach (var path in paths)
        {
            if (path.Value is not JsonObject pathItem)
            {
                continue;
            }

            foreach (var entry in pathItem)
            {
                if (entry.Value is not JsonObject operation)
                {
                    continue;
                }

                if (operation["responses"] is not JsonObject responses)
                {
                    continue;
                }

                foreach (var definition in options.Definitions)
                {
                    ApplyDefinition(responses, definition, options);
                }
            }
        }
    }

    /// <summary>
    /// Applies one response definition to the responses of a single operation.
    /// </summary>
    /// <param name="responses">The operation responses.</param>
    /// <param name="definition">The response definition.</param>
    /// <param name="options">The response catalog.</param>
    private static void ApplyDefinition(
        JsonObject responses,
        ProblemResponseDefinition definition,
        OpenApiProblemResponsesOptions options)
    {
        var reference = new JsonObject
        {
            ["$ref"] = $"#/components/responses/{definition.ComponentName}"
        };

        if (!responses.TryGetPropertyValue(definition.StatusCode, out var existing))
        {
            if (definition.IncludeWhenMissing)
            {
                responses[definition.StatusCode] = reference;
            }

            return;
        }

        var replace = definition.Replacement switch
        {
            ProblemResponseReplacementMode.Always => true,
            ProblemResponseReplacementMode.WhenContentMatchesProblemDetails =>
                IsProblemResponse(existing, options),
            _ => false
        };

        if (replace)
        {
            responses[definition.StatusCode] = reference;
        }
    }

    /// <summary>
    /// Determines whether an existing response already describes a problem response.
    /// </summary>
    /// <param name="node">The existing response node.</param>
    /// <param name="options">The response catalog.</param>
    /// <returns>
    /// <see langword="true"/> when the response has no content, uses the configured problem
    /// content type, or references one of the configured problem schemas.
    /// </returns>
    private static bool IsProblemResponse(
        JsonNode? node,
        OpenApiProblemResponsesOptions options)
    {
        if (node is not JsonObject response)
        {
            return false;
        }

        if (response.ContainsKey("$ref"))
        {
            return false;
        }

        if (!response.TryGetPropertyValue("content", out var contentNode)
            || contentNode is not JsonObject content)
        {
            return true;
        }

        if (content.ContainsKey(options.ContentType))
        {
            return true;
        }

        foreach (var mediaType in content)
        {
            if (ReferencesProblemSchema(mediaType.Value, options))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Determines whether a media type references one of the configured problem schemas.
    /// </summary>
    /// <param name="node">The media type node.</param>
    /// <param name="options">The response catalog.</param>
    /// <returns>
    /// <see langword="true"/> when the schema reference ends with a problem schema name.
    /// </returns>
    private static bool ReferencesProblemSchema(
        JsonNode? node,
        OpenApiProblemResponsesOptions options)
    {
        if (node is not JsonObject mediaType)
        {
            return false;
        }

        if (!mediaType.TryGetPropertyValue("schema", out var schemaNode)
            || schemaNode is not JsonObject schema)
        {
            return false;
        }

        if (!schema.TryGetPropertyValue("$ref", out var referenceNode)
            || referenceNode is not JsonValue referenceValue
            || !referenceValue.TryGetValue<string>(out var reference))
        {
            return false;
        }

        foreach (var schemaId in options.ProblemSchemaIds)
        {
            if (reference.EndsWith(string.Concat("/", schemaId), StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Converts an additional example field into a JSON node.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <returns>The converted node, or <see langword="null"/>.</returns>
    private static JsonNode? ToJsonNode(object? value)
    {
        if (value is null)
        {
            return null;
        }

        return value is JsonNode node
            ? node
            : JsonSerializer.SerializeToNode(value, value.GetType());
    }
}

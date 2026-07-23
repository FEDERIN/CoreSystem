namespace Core.Http.Responses;

/// <summary>
/// Represents a captured HTTP response, including the response body,
/// status code, and headers.
/// </summary>
public sealed record CapturedResponse
{
    /// <summary>
    /// Gets the response body as a byte array.
    /// </summary>
    public required byte[] Body { get; init; }

    /// <summary>
    /// Gets the HTTP status code.
    /// </summary>
    public required int StatusCode { get; init; }

    /// <summary>
    /// 
    /// </summary>
    public string? ContentType { get; init; }

    /// <summary>
    /// Gets the response headers captured from the original response.
    /// </summary>
    public required IReadOnlyDictionary<string, string[]> Headers
    {
        get;
        init;
    }
}
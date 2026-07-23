namespace Core.Idempotency.Internal.Capture;

internal sealed class CapturedResponse
{
    public required int StatusCode { get; init; }

    public string? ContentType { get; init; }

    public required string Body { get; init; }

    public required IReadOnlyDictionary<string, string[]> Headers { get; init; }
}
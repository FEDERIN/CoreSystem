namespace Core.DistributedCache.Http;

public sealed record CapturedResponse
{
    public required byte[] Body { get; init; }
    public required int StatusCode { get; init; }
    public Dictionary<string, string[]> Headers { get; init; } = [];
}
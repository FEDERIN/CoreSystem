namespace Core.DistributedCache.Http;

using Microsoft.Extensions.Primitives;

internal sealed record CachedHttpResponse
{
    public required byte[] Body { get; init; }
    public required int StatusCode { get; init; }
    public Dictionary<string, string[]> Headers { get; init; } = [];
}
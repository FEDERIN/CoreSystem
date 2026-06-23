using Microsoft.AspNetCore.Http;

namespace Core.DistributedCache.Extensions;

internal static class HttpExtensions
{
    public static string GenerateCacheKey(this HttpRequest request, string prefix = "cache")
    {
        var sortedQuery = request.Query
            .OrderBy(q => q.Key)
            .Select(q => $"{q.Key}={q.Value}");

        var queryString = string.Join("&", sortedQuery);
        return $"{prefix}:{request.Path}:{queryString}"; // Simplificado para mayor consistencia
    }
}
using Microsoft.AspNetCore.Http;

namespace Core.RateLimiting.Abstractions;

public interface IRateLimitPartitionKeyResolver {
    string Resolve(HttpContext context);
}

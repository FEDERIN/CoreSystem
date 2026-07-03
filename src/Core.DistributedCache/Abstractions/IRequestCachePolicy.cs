using Microsoft.AspNetCore.Http;

namespace Core.DistributedCache.Abstractions;

internal interface IRequestCachePolicy
{
    bool CanCache(HttpContext context);
}
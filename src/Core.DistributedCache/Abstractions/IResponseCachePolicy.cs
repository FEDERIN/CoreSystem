using Microsoft.AspNetCore.Http;

namespace Core.DistributedCache.Abstractions;

internal interface IResponseCachePolicy
{
    bool CanCache(HttpContext context);
}
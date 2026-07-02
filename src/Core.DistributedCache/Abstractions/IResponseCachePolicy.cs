using Microsoft.AspNetCore.Http;

namespace Core.DistributedCache.Abstractions;

public interface IResponseCachePolicy
{
    bool CanCache(HttpContext context);
}
using Microsoft.AspNetCore.Http;

namespace Core.DistributedCache.Abstractions;

public interface IRequestCachePolicy
{
    bool CanCache(HttpContext context);
}
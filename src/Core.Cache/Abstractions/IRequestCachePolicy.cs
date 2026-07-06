using Microsoft.AspNetCore.Http;

namespace Core.Cache.Abstractions;

internal interface IRequestCachePolicy
{
    bool CanCache(HttpContext context);
}
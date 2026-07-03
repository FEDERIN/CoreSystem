using Microsoft.AspNetCore.Http;

namespace Core.Cache.Abstractions;

internal interface IResponseCachePolicy
{
    bool CanCache(HttpContext context);
}
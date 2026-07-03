using Microsoft.AspNetCore.Http;

namespace Core.DistributedCache.Abstractions;

internal interface ICacheKeyGenerator
{
    string Generate(HttpContext context);
}
using Microsoft.AspNetCore.Http;

namespace Core.Cache.Abstractions;

internal interface ICacheKeyGenerator
{
    string Generate(HttpContext context);
}
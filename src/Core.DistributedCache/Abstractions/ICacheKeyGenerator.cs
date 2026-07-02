using Microsoft.AspNetCore.Http;

namespace Core.DistributedCache.Abstractions;

public interface ICacheKeyGenerator
{
    string Generate(HttpContext context);
}
using Core.DistributedCache.Http.Caching;
using Microsoft.AspNetCore.Http;

namespace Core.DistributedCache.Abstractions;

public interface IResponseCapture
{
    Task<CapturedResponse> CaptureAsync(
        HttpContext context,
        RequestDelegate next);
}
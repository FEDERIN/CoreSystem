using Core.Cache.Http.Caching;
using Microsoft.AspNetCore.Http;

namespace Core.Cache.Abstractions;

internal interface IResponseCapture
{
    Task<CapturedResponse> CaptureAsync(
        HttpContext context,
        RequestDelegate next);
}
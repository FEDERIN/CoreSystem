using Core.Idempotency.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Core.Idempotency.KeyResolution;

internal sealed class HeaderIdempotencyKeyResolver
    : IIdempotencyKeyResolver
{
    public bool TryResolve(
        HttpContext context,
        out string key)
    {
        var idempotency =
            context.Request.Headers["Idempotency-Key"];

        if (StringValues.IsNullOrEmpty(idempotency))
        {
            key = string.Empty;
            return false;
        }

        key =
            $"{context.Request.Method}:{context.Request.Path}:{idempotency}";

        return true;
    }
}
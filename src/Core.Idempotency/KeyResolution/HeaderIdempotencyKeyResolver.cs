using Core.Idempotency.Abstractions;
using Core.Idempotency.Constants;
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
            context.Request.Headers[HeaderNames.IdempotencyKey];

        if (StringValues.IsNullOrEmpty(idempotency))
        {
            key = string.Empty;
            return false;
        }

        key = idempotency.ToString();

        return true;
    }
}
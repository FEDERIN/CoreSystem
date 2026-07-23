using System.Collections.Frozen;
using Microsoft.Net.Http.Headers;

namespace Core.Http.Responses;

internal static class IgnoredResponseHeaders
{
    public static readonly FrozenSet<string> Values =
        new[]
        {
            HeaderNames.TransferEncoding,
            HeaderNames.ContentLength,
            HeaderNames.Connection,
            HeaderNames.Date,
            HeaderNames.Server,
            HeaderNames.KeepAlive,
            HeaderNames.Upgrade,
            HeaderNames.TE,
            HeaderNames.Trailer,
            HeaderNames.ProxyAuthenticate,
            HeaderNames.ProxyAuthorization
        }.ToFrozenSet(StringComparer.OrdinalIgnoreCase);
}
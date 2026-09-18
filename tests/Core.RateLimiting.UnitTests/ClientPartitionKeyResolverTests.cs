using Core.RateLimiting.Internal;
using Core.RateLimiting.Options;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Security.Claims;

namespace Core.RateLimiting.UnitTests;

public sealed class ClientPartitionKeyResolverTests
{
    [Fact]
    public void Resolve_PrefersAuthenticatedSubject()
    {
        var context = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity([new Claim("sub", "user-123")], "test"))
        };
        context.Connection.RemoteIpAddress = IPAddress.Parse("203.0.113.10");

        new ClientPartitionKeyResolver(new RateLimitingOptions { SubjectClaimType = "sub" })
            .Resolve(context).Should().Be("subject:user-123");
    }

    [Fact]
    public void Resolve_FallsBackToRemoteIpAddress()
    {
        var context = new DefaultHttpContext();
        context.Connection.RemoteIpAddress = IPAddress.Parse("203.0.113.10");

        new ClientPartitionKeyResolver(new RateLimitingOptions { SubjectClaimType = "sub" })
            .Resolve(context).Should().Be("ip:203.0.113.10");
    }
}

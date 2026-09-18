using Core.RateLimiting.DependencyInjection;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace Core.RateLimiting.UnitTests;

public sealed class RateLimitingMiddlewareTests
{
    [Fact]
    public async Task GlobalLimiter_ReturnsProblemDetailsAndRetryAfter_WhenPermitIsExhausted()
    {
        using var server = new TestServer(new WebHostBuilder()
            .ConfigureServices(services => services.AddCoreRateLimiting(options =>
            {
                options.PermitLimit = 1;
                options.Window = TimeSpan.FromMinutes(1);
                options.SubjectClaimType = "sub";
            }))
            .Configure(app =>
            {
                app.UseCoreRateLimiting();
                app.Run(context => context.Response.WriteAsync("accepted"));
            }));

        using var first = await server.CreateClient().GetAsync("/", TestContext.Current.CancellationToken);
        using var rejected = await server.CreateClient().GetAsync("/", TestContext.Current.CancellationToken);

        first.StatusCode.Should().Be(HttpStatusCode.OK);
        rejected.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
        rejected.Content.Headers.ContentType?.MediaType.Should().Be("application/problem+json");
        rejected.Headers.RetryAfter.Should().NotBeNull();
        (await rejected.Content.ReadAsStringAsync(TestContext.Current.CancellationToken)).Should().Contain("retryAfterSeconds");
    }
}

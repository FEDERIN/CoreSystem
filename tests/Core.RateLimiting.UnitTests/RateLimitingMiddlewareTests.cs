using Core.RateLimiting.DependencyInjection;
using Core.RateLimiting.Options;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;

namespace Core.RateLimiting.UnitTests;

public sealed class RateLimitingMiddlewareTests
{
    [Fact]
    public async Task GlobalLimiter_ReturnsProblemDetailsAndRetryAfter_WhenPermitIsExhausted()
    {
        using IHost host = CreateHost(options =>
        {
            options.PermitLimit = 1;
            options.Window = TimeSpan.FromMinutes(1);
            options.SubjectClaimType = "sub";
        });
        using HttpClient client = host.GetTestServer().CreateClient();

        using var first = await client.GetAsync("/", TestContext.Current.CancellationToken);
        using var rejected = await client.GetAsync("/", TestContext.Current.CancellationToken);

        first.StatusCode.Should().Be(HttpStatusCode.OK);
        rejected.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
        rejected.Content.Headers.ContentType?.MediaType.Should().Be("application/problem+json");
        rejected.Headers.RetryAfter.Should().NotBeNull();
        (await rejected.Content.ReadAsStringAsync(TestContext.Current.CancellationToken)).Should().Contain("retryAfterSeconds");
    }

    [Fact]
    public async Task DisabledRateLimiting_AllowsRequestsWithoutRegisteringLimiter()
    {
        using IHost host = CreateHost(options => options.Enabled = false);
        using HttpClient client = host.GetTestServer().CreateClient();

        using var response = await client.GetAsync("/", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // HostBuilder + ConfigureWebHost instead of WebHostBuilder + new TestServer(IWebHostBuilder):
    // both are obsolete (ASPDEPR004/ASPDEPR008) on the ASP.NET Core 10 TestHost, and warnings are errors.
    private static IHost CreateHost(Action<RateLimitingOptions> configure) =>
        new HostBuilder()
            .ConfigureWebHost(webHostBuilder =>
            {
                webHostBuilder.UseTestServer();
                webHostBuilder.ConfigureServices(services => services.AddCoreRateLimiting(configure));
                webHostBuilder.Configure(app =>
                {
                    app.UseCoreRateLimiting();
                    app.Run(context => context.Response.WriteAsync("accepted"));
                });
            })
            .Start();
}

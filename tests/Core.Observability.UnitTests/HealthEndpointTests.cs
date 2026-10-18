using System.Net;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Core.Observability.UnitTests;

/// <summary>
/// End-to-end tests for <see cref="ObservabilityDependencyInjection.UseObservabilityEndpoints"/> against a
/// real in-memory server. These lock the readiness contract: <c>/health</c> is a liveness probe that never
/// reports on dependencies, <c>/ready</c> aggregates every registered health check.
/// </summary>
public sealed class HealthEndpointTests
{
    private const string Environment = "Testing";
    private const string ServiceName = "core-observability-tests";
    private const string ServiceNamespace = "CoreSystem.Tests";

    [Fact]
    public async Task Health_ReturnsOk_AndIgnoresRegisteredChecks()
    {
        await using ObservabilityTestServer server = await ObservabilityTestServer.StartAsync(
            services => services.AddHealthChecks().AddCheck("never-runs", () => HealthCheckResult.Unhealthy("boom")));
        using HttpClient client = server.CreateClient();

        using HttpResponseMessage response = await client.GetAsync("/health", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        JsonElement payload = await ReadPayload(response);
        payload.GetProperty("status").GetString().Should().Be("Healthy");
        payload.GetProperty("checks").EnumerateArray().Should().BeEmpty();
    }

    [Fact]
    public async Task Ready_ReturnsOk_WhenEveryCheckIsHealthy()
    {
        await using ObservabilityTestServer server = await ObservabilityTestServer.StartAsync(
            services => services.AddHealthChecks().AddCheck("up", () => HealthCheckResult.Healthy()));

        using HttpResponseMessage response =
            await server.CreateClient().GetAsync("/ready", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        JsonElement payload = await ReadPayload(response);
        payload.GetProperty("checks")[0].GetProperty("name").GetString().Should().Be("up");
        payload.GetProperty("checks")[0].GetProperty("status").GetString().Should().Be("Healthy");
    }

    [Fact]
    public async Task Ready_ReturnsServiceUnavailable_WhenACheckIsUnhealthy()
    {
        await using ObservabilityTestServer server = await ObservabilityTestServer.StartAsync(
            services => services.AddHealthChecks().AddCheck("down", () => HealthCheckResult.Unhealthy("no database")));

        using HttpResponseMessage response =
            await server.CreateClient().GetAsync("/ready", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
        JsonElement payload = await ReadPayload(response);
        payload.GetProperty("status").GetString().Should().Be("Unhealthy");
        payload.GetProperty("checks")[0].GetProperty("status").GetString().Should().Be("Unhealthy");
    }

    [Fact]
    public async Task Ready_ReportsEveryRegisteredCheck()
    {
        await using ObservabilityTestServer server = await ObservabilityTestServer.StartAsync(
            services => services.AddHealthChecks()
                .AddCheck("redis", () => HealthCheckResult.Healthy())
                .AddCheck("sql", () => HealthCheckResult.Degraded("slow")));

        using HttpResponseMessage response =
            await server.CreateClient().GetAsync("/ready", TestContext.Current.CancellationToken);

        JsonElement payload = await ReadPayload(response);
        payload.GetProperty("status").GetString().Should().Be("Degraded");
        payload.GetProperty("checks").EnumerateArray()
            .Select(check => check.GetProperty("name").GetString())
            .Should().BeEquivalentTo(["redis", "sql"]);
    }

    [Fact]
    public async Task Health_Response_IsJsonWithStatusAndTotalDuration()
    {
        await using ObservabilityTestServer server = await ObservabilityTestServer.StartAsync();

        using HttpResponseMessage response =
            await server.CreateClient().GetAsync("/health", TestContext.Current.CancellationToken);

        response.Content.Headers.ContentType!.MediaType.Should().Be("application/json");
        response.Content.Headers.ContentType.CharSet.Should().Be("utf-8");

        JsonElement payload = await ReadPayload(response);
        payload.TryGetProperty("totalDurationMs", out JsonElement duration).Should().BeTrue();
        duration.GetDouble().Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task Health_StaysOk_EvenWhenReadinessChecksFail()
    {
        await using ObservabilityTestServer server = await ObservabilityTestServer.StartAsync(
            services => services.AddHealthChecks().AddCheck("down", () => HealthCheckResult.Unhealthy()));

        using HttpClient client = server.CreateClient();

        using HttpResponseMessage health = await client.GetAsync("/health", TestContext.Current.CancellationToken);
        using HttpResponseMessage ready = await client.GetAsync("/ready", TestContext.Current.CancellationToken);

        health.StatusCode.Should().Be(HttpStatusCode.OK);
        ready.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
    }

    [Fact]
    public async Task HealthEndpoints_AreRegistered_WhenTelemetryIsDisabled()
    {
        await using ObservabilityTestServer server = await ObservabilityTestServer.StartAsync(
            configure: null,
            settings:
            [
                new("OpenTelemetry:Tracing:Enabled", "false"),
                new("OpenTelemetry:Metrics:Enabled", "false")
            ]);

        using HttpResponseMessage response =
            await server.CreateClient().GetAsync("/health", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UnknownRoute_ReturnsNotFound_SoHealthChecksAreNotTheOnlyEndpoint()
    {
        await using ObservabilityTestServer server = await ObservabilityTestServer.StartAsync();

        using HttpResponseMessage response =
            await server.CreateClient().GetAsync("/not-a-route", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private static async Task<JsonElement> ReadPayload(HttpResponseMessage response)
    {
        string json = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        using JsonDocument document = JsonDocument.Parse(json);
        return document.RootElement.Clone();
    }

    private sealed class ObservabilityTestServer : IAsyncDisposable
    {
        private readonly WebApplication _app;

        private ObservabilityTestServer(WebApplication app) => _app = app;

        public static async Task<ObservabilityTestServer> StartAsync(
            Action<IServiceCollection>? configure = null,
            params KeyValuePair<string, string?>[] settings)
        {
            WebApplicationBuilder builder = ObservabilityTestBuilder.Create(settings);
            builder.WebHost.UseTestServer();

            builder.AddObservability(Environment, ServiceName, ServiceNamespace);
            configure?.Invoke(builder.Services);

            WebApplication app = builder.Build();
            app.UseObservabilityEndpoints();
            await app.StartAsync(TestContext.Current.CancellationToken);

            return new ObservabilityTestServer(app);
        }

        public HttpClient CreateClient() => _app.GetTestClient();

        public async ValueTask DisposeAsync() => await _app.DisposeAsync();
    }
}
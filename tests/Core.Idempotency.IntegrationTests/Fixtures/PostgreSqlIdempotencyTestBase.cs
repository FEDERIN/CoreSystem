using Core.Idempotency.Abstractions;
using Core.Idempotency.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Idempotency.IntegrationTests.Fixtures;

public abstract class PostgreSqlIdempotencyTestBase : IDisposable
{
    private readonly TestServer _server;

    protected IServiceProvider Services { get; }

    protected HttpClient Client { get; }

    protected PostgreSqlIdempotencyTestBase(
        PostgreSqlContainerFixture fixture,
        Action<IEndpointRouteBuilder> configureEndpoints)
    {
        var builder = new WebHostBuilder();

        builder.ConfigureServices(services =>
        {
            services.AddRouting();
            services.AddCoreIdempotency(options =>
            {
                options.Enabled = true;
                options.Provider = IdempotencyProviderType.PostgreSQL;

                options.PostgreSql.ConnectionString = fixture.ConnectionString;
                options.Fingerprint.Enabled = true;
                options.Fingerprint.IncludeQueryString = true;
                options.Fingerprint.IncludeContentType = true;
                options.Fingerprint.IncludedHeaders.Add("Authorization");
            });
            services.AddProblemDetails();
        });

        builder.Configure(app =>
        {
            app.UseExceptionHandler();
            app.UseRouting();

            app.UseCoreIdempotency();

            app.UseEndpoints(configureEndpoints);
        });

        _server = new TestServer(builder);

        Client = _server.CreateClient();

        Services = _server.Services;
    }

    public void Dispose()
    {
        Client.Dispose();
        _server.Dispose();

        GC.SuppressFinalize(this);
    }
}
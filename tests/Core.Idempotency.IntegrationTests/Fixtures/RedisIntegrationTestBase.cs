using Core.Idempotency.Abstractions;
using Core.Idempotency.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Idempotency.IntegrationTests.Fixtures;

public abstract class RedisIdempotencyTestBase //: IAsyncDisposable
{
    //private readonly TestServer _server;

    //protected IServiceProvider Services { get; }

    //protected HttpClient Client { get; }

    //protected IConnectionMultiplexer Connection { get; }

    //protected IDatabase Database => Connection.GetDatabase();

    //protected RedisIdempotencyTestBase(
    //    RedisContainerFixture fixture,
    //    Action<IEndpointRouteBuilder> configureEndpoints)
    //{
    //    Connection = ConnectionMultiplexer.Connect(
    //        fixture.ConnectionString);

    //    var builder = new WebHostBuilder();

    //    builder.ConfigureServices(services =>
    //    {
    //        services.AddRouting();

    //        services.AddCoreIdempotency(options =>
    //        {
    //            options.Enabled = true;
    //            options.Provider = IdempotencyProviderType.Redis;

    //            options.Redis.Configuration = configuration =>
    //            {
    //                configuration.EndPoints.Add(
    //                    fixture.ConnectionString);
    //            };
    //        });

    //        services.AddProblemDetails();
    //    });

    //    builder.Configure(app =>
    //    {
    //        app.UseExceptionHandler();
    //        app.UseRouting();

    //        app.UseCoreIdempotency();

    //        app.UseEndpoints(configureEndpoints);
    //    });

    //    _server = new TestServer(builder);

    //    Client = _server.CreateClient();

    //    Services = _server.Services;
    //}

    //public async ValueTask DisposeAsync()
    //{
    //    Client.Dispose();

    //    _server.Dispose();

    //    await Connection.DisposeAsync();

    //    GC.SuppressFinalize(this);
    //}
}
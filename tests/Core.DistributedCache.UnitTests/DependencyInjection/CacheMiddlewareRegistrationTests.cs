using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.TestHost;
using Core.Cache.DependencyInjection;


namespace Core.Cache.UnitTests.DependencyInjection;

public class CacheMiddlewareRegistrationTests
{
    [Fact]
    public async Task UseCoreDistributedCache_RegistersMiddlewareInPipeline()
    {
        var builder = new WebHostBuilder()
            .ConfigureServices(services => {
                services.AddLogging();
                services.AddCoreDistributedCache(options => { });
            })
            .Configure(app => {
                app.UseCoreDistributedCache();
            });

        using var server = new TestServer(builder);

        Assert.NotNull(server.Host);
    }
}
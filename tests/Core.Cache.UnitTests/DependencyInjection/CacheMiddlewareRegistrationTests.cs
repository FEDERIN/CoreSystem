using Core.Cache.DependencyInjection;
using Microsoft.AspNetCore.Builder;

namespace Core.Cache.UnitTests.DependencyInjection;

public class CacheMiddlewareRegistrationTests
{
    [Fact]
    public async Task UseCoreCache_RegistersMiddlewareInPipeline()
    {
        var builder = WebApplication.CreateBuilder();

        builder.Services.AddCoreCache(options => { });

        var app = builder.Build();

        app.UseCoreCache();

        await app.StartAsync(TestContext.Current.CancellationToken);

        Assert.NotNull(app.Services);

        await app.StopAsync(TestContext.Current.CancellationToken);
    }

    [Fact]
    public void UseCoreCache_WithoutRegistration_ShouldThrow()
    {
        var builder = WebApplication.CreateBuilder();

        var app = builder.Build();

        var exception = Assert.Throws<InvalidOperationException>(() =>
        {
            app.UseCoreCache();
        });

        Assert.Equal(CacheMessages.MissingRegistration, exception.Message);
    }
}
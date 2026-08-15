using Core.Cache.DependencyInjection;
using Core.Cache.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

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

    [Fact]
    public void UseCoreCache_ShouldReturnApp_WhenCacheIsDisabled()
    {
        var builder = WebApplication.CreateBuilder();

        builder.Services.AddCoreCache(options =>
        {
            options.Enabled = false;
        });

        var app = builder.Build();

        var result = app.UseCoreCache();

        Assert.Same(app, result);
    }

    [Fact]
    public void UseCoreCache_WhenHandlerIsNotRegistered_ShouldThrow()
    {
        var builder = WebApplication.CreateBuilder();

        builder.Services.AddSingleton(
            new CacheOptions
            {
                Enabled = true
            });

        var app = builder.Build();

        var exception = Assert.Throws<InvalidOperationException>(
            () => app.UseCoreCache());

        Assert.Equal(
            CacheMessages.MissingRegistration,
            exception.Message);
    }
}
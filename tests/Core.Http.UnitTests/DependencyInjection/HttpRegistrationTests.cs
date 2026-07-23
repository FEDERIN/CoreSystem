using Core.Http.Abstractions;
using Core.Http.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Http.UnitTests.DependencyInjection;

public class HttpRegistrationTests
{
    [Fact]
    public void AddCoreHttp_ShouldRegisterSingletonServices()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddCoreHttp();

        // Assert
        Assert.Contains(
            services,
            x => x.ServiceType == typeof(IResponseCapture)
              && x.Lifetime == ServiceLifetime.Singleton);

        Assert.Contains(
            services,
            x => x.ServiceType == typeof(IHttpResponseWriter)
              && x.Lifetime == ServiceLifetime.Singleton);
    }

    [Fact]
    public void AddCoreHttp_ShouldResolveRegisteredServices()
    {
        // Arrange
        var services = new ServiceCollection();

        services.AddCoreHttp();

        using var provider = services.BuildServiceProvider();

        // Act & Assert
        Assert.NotNull(provider.GetRequiredService<IResponseCapture>());
        Assert.NotNull(provider.GetRequiredService<IHttpResponseWriter>());
    }
}
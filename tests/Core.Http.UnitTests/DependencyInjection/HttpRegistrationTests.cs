using Core.Http.Abstractions;
using Core.Http.DependencyInjection;
using Core.Http.ProblemDetails;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Http.UnitTests.DependencyInjection;

public sealed class HttpRegistrationTests
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

public sealed class ProblemDetailsRegistrationTests
{
    [Fact]
    public void AddCoreExceptionHandler_RegistersAndResolvesTheMapper()
    {
        var services = new ServiceCollection();
        services.AddCoreProblemDetails();

        services.AddCoreExceptionHandler<TestMapper>();

        Assert.Contains(services, service =>
            service.ServiceType == typeof(Microsoft.AspNetCore.Diagnostics.IExceptionHandler));
        Assert.Contains(services, service =>
            service.ServiceType == typeof(TestMapper)
            && service.Lifetime == ServiceLifetime.Scoped);
    }

    private sealed class TestMapper : IExceptionProblemMapper
    {
        public ProblemDescriptor Map(Exception exception) => new(500, "TEST", "Test", "Test");
    }
}

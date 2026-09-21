using Core.Http.ProblemDetails.DependencyInjection;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;

namespace Core.Http.ProblemDetails.UniTests.DependencyInjection;

public sealed class HttpProblemDetailsRegistrationTests
{
    [Fact]
    public void AddCoreExceptionHandler_RegistersAScopedMapper_AndResolvesTheSingletonHandlerWithScopeValidation()
    {
        var services = new ServiceCollection();
        services.AddOptions();
        services.AddSingleton<IHostEnvironment>(Mock.Of<IHostEnvironment>());
        services.AddCoreProblemDetails();

        services.AddCoreExceptionHandler<TestMapper>();

        Assert.Contains(services, service =>
            service.ServiceType == typeof(Microsoft.AspNetCore.Diagnostics.IExceptionHandler));
        Assert.Contains(services, service =>
            service.ServiceType == typeof(TestMapper)
            && service.Lifetime == ServiceLifetime.Scoped);

        using ServiceProvider provider = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateScopes = true
        });

        IExceptionHandler handler = provider.GetRequiredService<IExceptionHandler>();
        Assert.NotNull(handler);
    }

    private sealed class TestMapper : IExceptionProblemMapper
    {
        public ProblemDescriptor Map(Exception exception) => new(500, "TEST", "Test", "Test");
    }
}

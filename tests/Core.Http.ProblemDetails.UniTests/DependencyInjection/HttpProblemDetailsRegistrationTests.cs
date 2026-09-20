using Core.Http.ProblemDetails.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Http.ProblemDetails.UniTests.DependencyInjection;

public sealed class HttpProblemDetailsRegistrationTests
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

using System.Net;
using System.Net.Http.Json;
using System.Diagnostics;
using Core.Http.ProblemDetails.DependencyInjection;
using Core.Http.ProblemDetails.Handlers;
using Core.Http.ProblemDetails.Options;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Moq;

namespace Core.Http.ProblemDetails.UniTests;

public sealed class CoreExceptionHandlerTests
{
    [Theory]
    [InlineData("/bad-request", HttpStatusCode.BadRequest, "BAD_REQUEST")]
    [InlineData("/not-found", HttpStatusCode.NotFound, "NOT_FOUND")]
    [InlineData("/orders/42", HttpStatusCode.Conflict, "ORDER_EXISTS")]
    [InlineData("/failure", HttpStatusCode.InternalServerError, "INTERNAL_ERROR")]
    public async Task WritesMappedExceptionsAsProblemJson_WithCoreExtensions(
        string path,
        HttpStatusCode expectedStatus,
        string expectedErrorCode)
    {
        using var server = CreateServer(Environments.Production);

        using HttpResponseMessage response = await server.CreateClient().GetAsync(path, TestContext.Current.CancellationToken);
        ProblemResponse? problem = await response.Content.ReadFromJsonAsync<ProblemResponse>(TestContext.Current.CancellationToken);

        Assert.Equal(expectedStatus, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType!.MediaType);
        Assert.NotNull(problem);
        Assert.Equal(expectedErrorCode, problem.ErrorCode);
        Assert.Equal(path, problem.Instance);
        Assert.Equal("about:blank", problem.Type);
        Assert.False(string.IsNullOrWhiteSpace(problem.TraceId));
        Assert.Null(problem.ExceptionType);
        Assert.Null(problem.ExceptionMessage);
        Assert.Equal("value", problem.Custom);
    }

    [Fact]
    public async Task IncludesExceptionDetail_OnlyInDevelopment()
    {
        using var server = CreateServer(Environments.Development);

        using HttpResponseMessage response = await server.CreateClient().GetAsync("/", TestContext.Current.CancellationToken);
        ProblemResponse? problem = await response.Content.ReadFromJsonAsync<ProblemResponse>(TestContext.Current.CancellationToken);

        Assert.Equal("StatusException", problem!.ExceptionType);
        Assert.Equal("Sensitive implementation detail", problem.ExceptionMessage);
    }

    [Fact]
    public async Task UsesTheW3cTraceId_WhenAnActivityIsAvailable()
    {
        var problemDetailsService = new Mock<IProblemDetailsService>();
        Microsoft.AspNetCore.Mvc.ProblemDetails? writtenProblem = null;
        problemDetailsService
            .Setup(service => service.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
            .Callback<ProblemDetailsContext>(context => writtenProblem = context.ProblemDetails)
            .Returns(new ValueTask<bool>(true));

        var environment = new Mock<IHostEnvironment>();
        environment.SetupGet(host => host.EnvironmentName).Returns(Environments.Production);

        var services = new ServiceCollection();
        services.AddScoped<TestMapper>();
        await using ServiceProvider provider = services.BuildServiceProvider();
        var handler = new CoreExceptionHandler<TestMapper>(
            provider.GetRequiredService<IServiceScopeFactory>(),
            problemDetailsService.Object,
            environment.Object,
            Microsoft.Extensions.Options.Options.Create(new CoreProblemDetailsOptions()));
        var httpContext = new DefaultHttpContext();

        using var activity = new Activity("test").SetIdFormat(ActivityIdFormat.W3C).Start();
        await handler.TryHandleAsync(httpContext, new StatusException("/"), TestContext.Current.CancellationToken);

        Assert.NotNull(writtenProblem);
        Assert.Equal(activity.TraceId.ToString(), writtenProblem.Extensions["traceId"]);
    }

    [Fact]
    public async Task ReturnsFalseWithoutWriting_WhenTheResponseHasAlreadyStarted()
    {
        var problemDetailsService = new Mock<IProblemDetailsService>();
        using ServiceProvider provider = CreateServiceProvider();
        var handler = CreateHandler(provider, problemDetailsService.Object);
        var httpContext = new DefaultHttpContext();
        var responseFeature = new Mock<IHttpResponseFeature>();
        responseFeature.SetupGet(feature => feature.HasStarted).Returns(true);
        httpContext.Features.Set<IHttpResponseFeature>(responseFeature.Object);

        bool handled = await handler.TryHandleAsync(httpContext, new StatusException("/"), TestContext.Current.CancellationToken);

        Assert.False(handled);
        problemDetailsService.Verify(service => service.TryWriteAsync(It.IsAny<ProblemDetailsContext>()), Times.Never);
    }

    [Fact]
    public async Task ReturnsFalse_WhenTheProblemDetailsWriterCannotWriteTheResponse()
    {
        var problemDetailsService = new Mock<IProblemDetailsService>();
        problemDetailsService
            .Setup(service => service.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
            .Returns(new ValueTask<bool>(false));
        using ServiceProvider provider = CreateServiceProvider();
        var handler = CreateHandler(provider, problemDetailsService.Object);
        var httpContext = new DefaultHttpContext();

        bool handled = await handler.TryHandleAsync(httpContext, new StatusException("/"), TestContext.Current.CancellationToken);

        Assert.False(handled);
        Assert.Equal(StatusCodes.Status500InternalServerError, httpContext.Response.StatusCode);
    }

    [Fact]
    public async Task WritesPathBaseAndExplicitProblemType()
    {
        var problemDetailsService = new Mock<IProblemDetailsService>();
        Microsoft.AspNetCore.Mvc.ProblemDetails? writtenProblem = null;
        problemDetailsService
            .Setup(service => service.TryWriteAsync(It.IsAny<ProblemDetailsContext>()))
            .Callback<ProblemDetailsContext>(context => writtenProblem = context.ProblemDetails)
            .Returns(new ValueTask<bool>(true));
        using ServiceProvider provider = CreateServiceProvider();
        var handler = CreateHandler(provider, problemDetailsService.Object);
        var httpContext = new DefaultHttpContext();
        httpContext.Request.PathBase = "/api";
        httpContext.Request.Path = "/orders/42";

        await handler.TryHandleAsync(httpContext, new StatusException("/typed"), TestContext.Current.CancellationToken);

        Assert.NotNull(writtenProblem);
        Assert.Equal("/api/orders/42", writtenProblem.Instance);
        Assert.Equal("urn:test:error:typed", writtenProblem.Type);
    }

    [Fact]
    public async Task PropagatesFailuresFromTheCustomizationCallback()
    {
        var problemDetailsService = new Mock<IProblemDetailsService>();
        using ServiceProvider provider = CreateServiceProvider();
        var handler = CreateHandler(
            provider,
            problemDetailsService.Object,
            new CoreProblemDetailsOptions
            {
                CustomizeProblemDetails = (_, _, _) => throw new InvalidOperationException("Customization failed")
            });

        InvalidOperationException exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.TryHandleAsync(new DefaultHttpContext(), new StatusException("/"), TestContext.Current.CancellationToken).AsTask());

        Assert.Equal("Customization failed", exception.Message);
        problemDetailsService.Verify(service => service.TryWriteAsync(It.IsAny<ProblemDetailsContext>()), Times.Never);
    }

    private static TestServer CreateServer(string environment) => new(new WebHostBuilder()
        .UseEnvironment(environment)
        .ConfigureServices(services =>
        {
            services.AddCoreProblemDetails(options =>
                options.CustomizeProblemDetails = (problem, _, _) => problem.Extensions["custom"] = "value");
            services.AddCoreExceptionHandler<TestMapper>();
        })
        .Configure(app =>
        {
            app.UseExceptionHandler();
            app.Run(context => throw new StatusException(context.Request.Path));
        }));

    private static ServiceProvider CreateServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddScoped<TestMapper>();
        return services.BuildServiceProvider();
    }

    private static CoreExceptionHandler<TestMapper> CreateHandler(
        ServiceProvider provider,
        IProblemDetailsService problemDetailsService,
        CoreProblemDetailsOptions? options = null)
    {
        var environment = new Mock<IHostEnvironment>();
        environment.SetupGet(host => host.EnvironmentName).Returns(Environments.Production);
        return new CoreExceptionHandler<TestMapper>(
            provider.GetRequiredService<IServiceScopeFactory>(),
            problemDetailsService,
            environment.Object,
            Microsoft.Extensions.Options.Options.Create(options ?? new CoreProblemDetailsOptions()));
    }

    private sealed class TestMapper : IExceptionProblemMapper
    {
        public ProblemDescriptor Map(Exception exception)
        {
            var statusException = Assert.IsType<StatusException>(exception);
            return statusException.Path.Value switch
            {
                "/bad-request" => new(StatusCodes.Status400BadRequest, "BAD_REQUEST", "Bad request", "The request is invalid"),
                "/not-found" => new(StatusCodes.Status404NotFound, "NOT_FOUND", "Not found", "The resource was not found"),
                "/orders/42" => new(StatusCodes.Status409Conflict, "ORDER_EXISTS", "Order already exists", "An order with this identifier already exists"),
                "/typed" => new(StatusCodes.Status422UnprocessableEntity, "TYPED", "Typed", "Typed", "urn:test:error:typed"),
                _ => new(StatusCodes.Status500InternalServerError, "INTERNAL_ERROR", "Unexpected error", "Sensitive implementation detail")
            };
        }
    }

    private sealed class ProblemResponse
    {
        public string? ErrorCode { get; init; }
        public string? TraceId { get; init; }
        public string? Instance { get; init; }
        public string? Type { get; init; }
        public string? ExceptionType { get; init; }
        public string? ExceptionMessage { get; init; }
        public string? Custom { get; init; }
    }

    private sealed class StatusException(PathString path) : Exception("Sensitive implementation detail")
    {
        public PathString Path { get; } = path;
    }
}

using System.Net;
using System.Net.Http.Json;
using Core.Http.ProblemDetails.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

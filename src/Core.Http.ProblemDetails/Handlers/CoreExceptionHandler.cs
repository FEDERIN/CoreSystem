using System.Diagnostics;
using Core.Http.ProblemDetails.Options;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Core.Http.ProblemDetails.Handlers;

/// <summary>Writes mapped exceptions as RFC 9457 problem details.</summary>
public sealed class CoreExceptionHandler<TMapper>(
    TMapper mapper,
    IProblemDetailsService problemDetailsService,
    IHostEnvironment environment,
    IOptions<CoreProblemDetailsOptions> options) : IExceptionHandler
    where TMapper : class, IExceptionProblemMapper
{
    /// <summary>Handles the exception by serializing its mapped problem representation.</summary>
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (httpContext.Response.HasStarted)
        {
            return false;
        }

        httpContext.Response.Clear();
        ProblemDescriptor descriptor = mapper.Map(exception);
        var problem = new Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            Status = descriptor.Status,
            Title = descriptor.Title,
            Detail = descriptor.Detail,
            Type = descriptor.Type ?? "about:blank",
            Instance = $"{httpContext.Request.PathBase}{httpContext.Request.Path}"
        };

        problem.Extensions["errorCode"] = descriptor.ErrorCode;
        problem.Extensions["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        if (environment.IsDevelopment())
        {
            problem.Extensions["exceptionType"] = exception.GetType().Name;
            problem.Extensions["exceptionMessage"] = exception.Message;
        }

        options.Value.CustomizeProblemDetails?.Invoke(problem, httpContext, exception);
        httpContext.Response.StatusCode = descriptor.Status;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = problem
        });
    }
}

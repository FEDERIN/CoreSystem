using Core.Idempotency.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreSystem.Samples.Infrastructure.ExceptionHandling;

internal sealed class IdempotencyExceptionHandler(
    IProblemDetailsService problemDetailsService)
    : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService = problemDetailsService;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext context,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not IdempotencyFingerprintMismatchException)
        {
            return false;
        }

        var problem = new ProblemDetails
        {
            Type = IdempotencyFingerprintMismatchException.Type,
            Title = IdempotencyFingerprintMismatchException.Title,
            Status = StatusCodes.Status409Conflict,
            Detail = exception.Message
        };

        problem.Extensions["idempotencyKey"] =
            context.Request.Headers["Idempotency-Key"].ToString();
        context.Response.StatusCode = problem.Status.GetValueOrDefault();

        return await _problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = context,
                ProblemDetails = problem,
                Exception = exception
            });
    }
}
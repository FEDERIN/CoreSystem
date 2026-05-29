using Core.Idempotency.Middleware;
using Core.Idempotency.Options;
using Core.Idempotency.Diagnostics;
using Core.Idempotency.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Moq;

namespace Core.Idempotency.Tests.Fixtures;

public class IdempotencyMiddlewareFixture
{
    public Mock<IIdempotencyStorage> StorageMock { get; set; }
    public Mock<RequestDelegate> NextMock { get; set; }
    public IdempotencyOptions Options { get; set; }
    public IdempotencyMetrics Metrics { get; set; }
    public DefaultHttpContext HttpContext { get; set; }

    public IdempotencyMiddlewareFixture()
    {
        StorageMock = new Mock<IIdempotencyStorage>();
        NextMock = new Mock<RequestDelegate>();
        Options = new IdempotencyOptions
        {
            Enabled = true,
            HeaderName = "X-Idempotency-Key",
            AllowedMethods = ["POST", "PUT"],
            Expiration = TimeSpan.FromHours(1),
            MeterName = "test.idempotency"
        };
        Metrics = new IdempotencyMetrics(Options.MeterName);
        HttpContext = new DefaultHttpContext();
    }

    public IdempotencyMiddleware CreateMiddleware()
    {
        var optionsWrapper = Options.AsOptions();
        return new IdempotencyMiddleware(NextMock.Object, optionsWrapper, Metrics, StorageMock.Object);
    }
}

internal static class OptionsExtensions
{
    public static IOptions<T> AsOptions<T>(this T value) where T : class
    {
        return Microsoft.Extensions.Options.Options.Create(value);
    }
}
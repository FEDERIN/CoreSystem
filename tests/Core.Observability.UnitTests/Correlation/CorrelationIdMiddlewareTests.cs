using Core.Observability.Correlation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Core.Observability.UnitTests.Correlation;

public sealed class CorrelationIdMiddlewareTests
{
    [Fact]
    public async Task PreservesValidIncomingHeader_AndAddsItToContextAndResponse()
    {
        const string correlationId = "order-42.a";
        using var server = CreateServer(context =>
        {
            Assert.Equal(correlationId, context.Items[CoreCorrelationConstants.HttpContextItemKey]);
            return Task.CompletedTask;
        });
        using HttpClient client = server.CreateClient();
        client.DefaultRequestHeaders.Add("X-Correlation-Id", correlationId);

        using HttpResponseMessage response = await client.GetAsync("/", TestContext.Current.CancellationToken);

        Assert.Equal(correlationId, response.Headers.GetValues("X-Correlation-Id").Single());
    }

    [Fact]
    public async Task GeneratesGuid_WhenHeaderIsMissing()
    {
        string? correlationId = null;
        using var server = CreateServer(context =>
        {
            correlationId = context.Items[CoreCorrelationConstants.HttpContextItemKey] as string;
            return Task.CompletedTask;
        });

        using HttpResponseMessage response = await server.CreateClient().GetAsync("/", TestContext.Current.CancellationToken);

        Assert.True(Guid.TryParse(correlationId, out _));
        Assert.Equal(correlationId, response.Headers.GetValues("X-Correlation-Id").Single());
    }

    [Theory]
    [InlineData("invalid value")]
    [InlineData("this-value-is-deliberately-made-much-longer-than-the-maximum-correlation-identifier-length")]
    public async Task ReplacesInvalidOrExcessiveHeader(string value)
    {
        using var server = CreateServer(_ => Task.CompletedTask, options => options.MaximumLength = 24);
        using HttpClient client = server.CreateClient();
        client.DefaultRequestHeaders.Add("X-Correlation-Id", value);

        using HttpResponseMessage response = await client.GetAsync("/", TestContext.Current.CancellationToken);

        string actual = response.Headers.GetValues("X-Correlation-Id").Single();
        Assert.NotEqual(value, actual);
        Assert.True(Guid.TryParse(actual, out _));
    }

    [Fact]
    public async Task AddsCorrelationIdToLogScope()
    {
        var loggerProvider = new ScopeCapturingLoggerProvider();
        using var server = CreateServer(
            context =>
            {
                context.RequestServices.GetRequiredService<ILogger<CorrelationIdMiddlewareTests>>()
                    .LogInformation("Request handled");
                return Task.CompletedTask;
            },
            loggerProvider: loggerProvider);
        using HttpClient client = server.CreateClient();
        client.DefaultRequestHeaders.Add("X-Correlation-Id", "logged-request");

        using HttpResponseMessage response = await client.GetAsync("/", TestContext.Current.CancellationToken);

        Assert.Contains(loggerProvider.Scopes,
            scope => scope.TryGetValue(CoreCorrelationConstants.LogPropertyName, out object? value)
                && Equals(value, "logged-request"));
    }

    private static TestServer CreateServer(
        RequestDelegate endpoint,
        Action<CorrelationOptions>? configure = null,
        ScopeCapturingLoggerProvider? loggerProvider = null) =>
        new(new WebHostBuilder()
            .ConfigureServices(services =>
            {
                services.AddLogging(logging =>
                {
                    if (loggerProvider is not null)
                    {
                        logging.AddProvider(loggerProvider);
                    }
                });
                services.AddCoreCorrelation(configure);
            })
            .Configure(app => app.UseCoreCorrelationId().Run(endpoint)));

    private sealed class ScopeCapturingLoggerProvider : ILoggerProvider, ISupportExternalScope
    {
        private readonly List<Dictionary<string, object?>> _scopes = [];
        private IExternalScopeProvider _scopeProvider = new LoggerExternalScopeProvider();

        public IReadOnlyList<Dictionary<string, object?>> Scopes => _scopes;

        public ILogger CreateLogger(string categoryName) => new ScopeCapturingLogger(this);

        public void Dispose()
        {
        }

        public void SetScopeProvider(IExternalScopeProvider scopeProvider) => _scopeProvider = scopeProvider;

        private sealed class ScopeCapturingLogger(ScopeCapturingLoggerProvider provider) : ILogger
        {
            public IDisposable? BeginScope<TState>(TState state) where TState : notnull =>
                provider._scopeProvider.Push(state);

            public bool IsEnabled(LogLevel logLevel) => true;

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
                Func<TState, Exception?, string> formatter)
            {
                provider._scopeProvider.ForEachScope((scope, captured) =>
                {
                    if (scope is IEnumerable<KeyValuePair<string, object>> values)
                    {
                        captured.Add(values.ToDictionary(pair => pair.Key, pair => (object?)pair.Value));
                    }
                }, provider._scopes);
            }
        }
    }
}

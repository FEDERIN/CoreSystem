using Core.Observability.Abstractions;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Serilog.Extensions.Logging;

namespace Core.Observability.UnitTests;

/// <summary>
/// Behavioural tests for <see cref="ObservabilityDependencyInjection.AddObservability"/>. Everything is
/// asserted through the public surface because the Serilog, OpenTelemetry and health check extensions are
/// <c>internal</c>: the container and the host builder are the only observable contract.
/// </summary>
public sealed class ObservabilityDependencyInjectionTests
{
    private const string Environment = "Testing";
    private const string ServiceName = "core-observability-tests";
    private const string ServiceNamespace = "CoreSystem.Tests";

    [Fact]
    public void AddObservability_ReturnsTheSameBuilderInstance()
    {
        WebApplicationBuilder builder = ObservabilityTestBuilder.Create();

        WebApplicationBuilder result = builder.AddObservability(Environment, ServiceName, ServiceNamespace);

        result.Should().BeSameAs(builder);
    }

    [Fact]
    public void AddObservability_WithoutOpenTelemetrySection_RegistersHealthChecksOnly()
    {
        WebApplicationBuilder builder = ObservabilityTestBuilder.Create();

        builder.AddObservability(Environment, ServiceName, ServiceNamespace);

        builder.Services.Should().Contain(descriptor => descriptor.ServiceType == typeof(HealthCheckService));
        HasTracerProvider(builder.Services).Should().BeFalse();
        HasMeterProvider(builder.Services).Should().BeFalse();
    }

    [Fact]
    public void AddObservability_WhenTelemetryIsDisabled_DoesNotThrow()
    {
        WebApplicationBuilder builder = ObservabilityTestBuilder.Create(
            KeyValuePair.Create<string, string?>("OpenTelemetry:Tracing:Enabled", "false"),
            KeyValuePair.Create<string, string?>("OpenTelemetry:Metrics:Enabled", "false"),
            KeyValuePair.Create<string, string?>("OpenTelemetry:Logging:Enabled", "false"));

        Action act = () => builder.AddObservability(Environment, ServiceName, ServiceNamespace);

        act.Should().NotThrow();
    }

    [Theory]
    [InlineData("true")]
    [InlineData("false")]
    public void AddObservability_AlwaysRegistersHealthChecks_RegardlessOfTracingFlag(string enabled)
    {
        WebApplicationBuilder builder = ObservabilityTestBuilder.Create(
            KeyValuePair.Create<string, string?>($"OpenTelemetry:Tracing:Enabled", enabled));

        builder.AddObservability(Environment, ServiceName, ServiceNamespace);

        using ServiceProvider provider = builder.Services.BuildServiceProvider();
        provider.GetService<HealthCheckService>().Should().NotBeNull();
    }

    [Fact]
    public void AddObservability_WhenTracingIsEnabled_RegistersTracerProvider()
    {
        WebApplicationBuilder builder = ObservabilityTestBuilder.CreateWithTracingEnabled();

        builder.AddObservability(Environment, ServiceName, ServiceNamespace);

        HasTracerProvider(builder.Services).Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("not-a-valid-uri")]
    public void AddObservability_WhenTracingEndpointIsUnusable_KeepsTheTracerProvider(string endpoint)
    {
        WebApplicationBuilder builder = ObservabilityTestBuilder.CreateWithTracingEnabled(endpoint);

        Action act = () => builder.AddObservability(Environment, ServiceName, ServiceNamespace);

        act.Should().NotThrow();
        HasTracerProvider(builder.Services).Should().BeTrue();
    }

    [Theory]
    [InlineData("-1")]
    [InlineData("1.5")]
    public void AddObservability_WhenSamplingProbabilityIsOutOfRange_FallsBackToFullSampling(string probability)
    {
        WebApplicationBuilder builder =
            ObservabilityTestBuilder.CreateWithTracingEnabled(samplingProbability: probability);

        Action act = () => builder.AddObservability(Environment, ServiceName, ServiceNamespace);

        act.Should().NotThrow();
        HasTracerProvider(builder.Services).Should().BeTrue();
    }

    [Fact]
    public void AddObservability_WhenSamplingProbabilityIsNaN_FallsBackToFullSampling()
    {
        WebApplicationBuilder builder =
            ObservabilityTestBuilder.CreateWithTracingEnabled(samplingProbability: "NaN");

        Action act = () => builder.AddObservability(Environment, ServiceName, ServiceNamespace);

        act.Should().NotThrow();
        HasTracerProvider(builder.Services).Should().BeTrue();
    }

    [Fact]
    public void AddObservability_WhenSamplingProbabilityIsNotANumber_ThrowsAConfigurationBindingError()
    {
        // Documented limitation, not a desired behaviour: Serilog binds the whole "OpenTelemetry"
        // section (SerilogExtensions only reads Logging), so an unparsable double fails the bind
        // before OpenTelemetryTracingExtensions can apply its NaN/range guard.
        WebApplicationBuilder builder =
            ObservabilityTestBuilder.CreateWithTracingEnabled(samplingProbability: "not-a-number");

        Action act = () => builder.AddObservability(Environment, ServiceName, ServiceNamespace);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*SamplingProbability*");
    }

    [Fact]
    public void AddObservability_WhenMetricsIsEnabled_RegistersMeterProvider()
    {
        WebApplicationBuilder builder = ObservabilityTestBuilder.CreateWithMetricsEnabled();

        builder.AddObservability(Environment, ServiceName, ServiceNamespace);

        HasMeterProvider(builder.Services).Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("not-a-valid-uri")]
    public void AddObservability_WhenMetricsEndpointIsUnusable_KeepsTheMeterProvider(string endpoint)
    {
        WebApplicationBuilder builder = ObservabilityTestBuilder.CreateWithMetricsEnabled(endpoint);

        Action act = () => builder.AddObservability(Environment, ServiceName, ServiceNamespace);

        act.Should().NotThrow();
        HasMeterProvider(builder.Services).Should().BeTrue();
    }

    [Fact]
    public void AddObservability_WithNullServiceName_UsesFallbackValuesInsteadOfThrowing()
    {
        WebApplicationBuilder builder = ObservabilityTestBuilder.CreateWithTracingEnabled();

        Action act = () => builder.AddObservability(Environment, null!, null!);

        act.Should().NotThrow();
    }

    [Fact]
    public void AddObservability_ReplacesTheDefaultLoggerFactoryWithSerilog()
    {
        WebApplicationBuilder builder = ObservabilityTestBuilder.Create();

        builder.AddObservability(Environment, ServiceName, ServiceNamespace);

        using ServiceProvider provider = builder.Services.BuildServiceProvider();
        provider.GetRequiredService<ILoggerFactory>().Should().BeOfType<SerilogLoggerFactory>();
    }

    [Theory]
    [InlineData("http://otel-collector:4317")]
    [InlineData("")]
    [InlineData("not-a-valid-uri")]
    public void AddObservability_WhenLoggingSinkEndpointIsUnusable_StillWiresSerilog(string endpoint)
    {
        WebApplicationBuilder builder = ObservabilityTestBuilder.CreateWithLoggingEnabled(endpoint);

        Action act = () => builder.AddObservability(Environment, ServiceName, ServiceNamespace);

        act.Should().NotThrow();

        using ServiceProvider provider = builder.Services.BuildServiceProvider();
        provider.GetRequiredService<ILoggerFactory>().Should().BeOfType<SerilogLoggerFactory>();
    }

    [Fact]
    public void AddObservability_InvokesRegisteredHealthCheckContributor()
    {
        WebApplicationBuilder builder = ObservabilityTestBuilder.Create();
        var contributor = new RecordingHealthCheckContributor();
        builder.Services.AddSingleton<IHealthCheckContributor>(contributor);

        builder.AddObservability(Environment, ServiceName, ServiceNamespace);

        contributor.Calls.Should().Be(1);

        using ServiceProvider provider = builder.Services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<HealthCheckServiceOptions>>().Value;
        options.Registrations.Select(registration => registration.Name).Should().Contain("recording-check");
    }

    [Fact]
    public void AddObservability_DoesNotInvokeHealthCheckContributor_WhenRegisteredAsAType()
    {
        WebApplicationBuilder builder = ObservabilityTestBuilder.Create();
        builder.Services.AddSingleton<IHealthCheckContributor, RecordingHealthCheckContributor>();

        builder.AddObservability(Environment, ServiceName, ServiceNamespace);

        using ServiceProvider provider = builder.Services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<HealthCheckServiceOptions>>().Value;

        // The registries only discover instances, so a type registration is intentionally not picked up.
        options.Registrations.Should().BeEmpty();
    }

    [Fact]
    public void AddObservability_WhenMetricsIsEnabled_ConfiguresRegisteredObservabilityContributor()
    {
        WebApplicationBuilder builder = ObservabilityTestBuilder.CreateWithMetricsEnabled();
        var contributor = new RecordingObservabilityContributor();
        builder.Services.AddSingleton<IObservabilityContributor>(contributor);

        builder.AddObservability(Environment, ServiceName, ServiceNamespace);

        contributor.ConfigureCalls.Should().Be(1);

        using ServiceProvider provider = builder.Services.BuildServiceProvider();
        provider.GetService<RecordingMarker>().Should().NotBeNull();
    }

    [Fact]
    public void AddObservability_WhenTelemetryIsDisabled_DoesNotConfigureObservabilityContributor()
    {
        WebApplicationBuilder builder = ObservabilityTestBuilder.Create();
        var contributor = new RecordingObservabilityContributor();
        builder.Services.AddSingleton<IObservabilityContributor>(contributor);

        builder.AddObservability(Environment, ServiceName, ServiceNamespace);

        contributor.ConfigureCalls.Should().Be(0);
    }

    [Fact]
    public void AddObservability_DeduplicatesContributorInstancesRegisteredTwice()
    {
        WebApplicationBuilder builder = ObservabilityTestBuilder.CreateWithMetricsEnabled();
        var contributor = new RecordingObservabilityContributor();
        builder.Services.AddSingleton<IObservabilityContributor>(contributor);
        builder.Services.AddSingleton<IObservabilityContributor>(contributor);

        builder.AddObservability(Environment, ServiceName, ServiceNamespace);

        contributor.ConfigureCalls.Should().Be(1);
    }

    [Fact]
    public void AddObservability_AddsHealthCheckContributorsToTheReadinessCheckOnly()
    {
        WebApplicationBuilder builder = ObservabilityTestBuilder.Create();
        builder.Services.AddSingleton<IHealthCheckContributor>(new RecordingHealthCheckContributor());

        builder.AddObservability(Environment, ServiceName, ServiceNamespace);

        using ServiceProvider provider = builder.Services.BuildServiceProvider();
        provider.GetRequiredService<HealthCheckService>().Should().NotBeNull();
    }

    private static bool HasTracerProvider(IServiceCollection services) =>
        services.Any(descriptor => descriptor.ServiceType == typeof(TracerProvider));

    private static bool HasMeterProvider(IServiceCollection services) =>
        services.Any(descriptor => descriptor.ServiceType == typeof(MeterProvider));

    private sealed class RecordingHealthCheckContributor : IHealthCheckContributor
    {
        public int Calls { get; private set; }

        public void RegisterHealthChecks(IHealthChecksBuilder builder, IConfiguration configuration)
        {
            Calls++;
            builder.AddCheck("recording-check", () => HealthCheckResult.Healthy());
        }
    }

    private sealed class RecordingObservabilityContributor : IObservabilityContributor
    {
        public int ConfigureCalls { get; private set; }

        public IEnumerable<string> GetActivitySources() => ["Core.Tests.Recording"];

        public void ConfigureObservability(IServiceCollection services, IConfiguration configuration)
        {
            ConfigureCalls++;
            services.AddSingleton<RecordingMarker>();
        }
    }

    private sealed class RecordingMarker;
}
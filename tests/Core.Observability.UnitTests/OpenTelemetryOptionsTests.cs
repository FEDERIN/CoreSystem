using Core.Observability.Options;
using FluentAssertions;
using Microsoft.Extensions.Configuration;

namespace Core.Observability.UnitTests;

/// <summary>
/// Pins the public configuration contract of <c>OpenTelemetryOptions</c>. Consumers bind their
/// configuration to these properties, so a silent default change here is a breaking change for them.
/// </summary>
public sealed class OpenTelemetryOptionsTests
{
    private const string DefaultOtlpEndpoint = "http://otel-collector:4317";

    [Fact]
    public void SectionName_IsOpenTelemetry()
    {
        OpenTelemetryOptions.SectionName.Should().Be("OpenTelemetry");
    }

    [Fact]
    public void Defaults_ExposeNonNullSubOptions()
    {
        var options = new OpenTelemetryOptions();

        options.Logging.Should().NotBeNull();
        options.Tracing.Should().NotBeNull();
        options.Metrics.Should().NotBeNull();
    }

    [Fact]
    public void Defaults_DisableEveryExporter()
    {
        var options = new OpenTelemetryOptions();

        options.Logging.Enabled.Should().BeFalse();
        options.Tracing.Enabled.Should().BeFalse();
        options.Metrics.Enabled.Should().BeFalse();
    }

    [Fact]
    public void Defaults_PointEveryExporterAtTheOtlpCollector()
    {
        const string expected = "http://otel-collector:4317";
        var options = new OpenTelemetryOptions();

        options.Logging.OtlpEndpoint.Should().Be(expected);
        options.Tracing.OtlpEndpoint.Should().Be(expected);
        options.Metrics.OtlpEndpoint.Should().Be(expected);
    }

    [Fact]
    public void Defaults_SampleEveryTraceAndCaptureNoSqlStatements()
    {
        var options = new OpenTelemetryOptions();

        options.Tracing.SamplingProbability.Should().Be(1.0);
        options.Tracing.CaptureSqlStatements.Should().BeFalse();
    }

    [Fact]
    public void Defaults_RegisterNoCustomMeters()
    {
        new OpenTelemetryOptions().Metrics.Meters.Should().BeEmpty();
    }

    [Fact]
    public void Bind_ReadsEveryExporterFromTheOpenTelemetrySection()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["OpenTelemetry:Logging:Enabled"] = "true",
                ["OpenTelemetry:Logging:OtlpEndpoint"] = "http://logs:4317",
                ["OpenTelemetry:Tracing:Enabled"] = "true",
                ["OpenTelemetry:Tracing:OtlpEndpoint"] = "http://traces:4317",
                ["OpenTelemetry:Tracing:SamplingProbability"] = "0.25",
                ["OpenTelemetry:Tracing:CaptureSqlStatements"] = "true",
                ["OpenTelemetry:Metrics:Enabled"] = "true",
                ["OpenTelemetry:Metrics:OtlpEndpoint"] = "http://metrics:4317",
                ["OpenTelemetry:Metrics:Meters:0"] = "Core.Resilience",
                ["OpenTelemetry:Metrics:Meters:1"] = "Core.RateLimiting"
            })
            .Build();

        var options = new OpenTelemetryOptions();
        configuration.GetSection(OpenTelemetryOptions.SectionName).Bind(options);

        options.Logging.Enabled.Should().BeTrue();
        options.Logging.OtlpEndpoint.Should().Be("http://logs:4317");
        options.Tracing.Enabled.Should().BeTrue();
        options.Tracing.OtlpEndpoint.Should().Be("http://traces:4317");
        options.Tracing.SamplingProbability.Should().Be(0.25);
        options.Tracing.CaptureSqlStatements.Should().BeTrue();
        options.Metrics.Enabled.Should().BeTrue();
        options.Metrics.OtlpEndpoint.Should().Be("http://metrics:4317");
        options.Metrics.Meters.Should().Equal("Core.Resilience", "Core.RateLimiting");
    }

    [Fact]
    public void Bind_LeavesDefaultsInPlaceWhenTheSectionIsAbsent()
    {
        IConfiguration configuration = new ConfigurationBuilder().Build();

        var options = new OpenTelemetryOptions();
        var section = configuration.GetSection(OpenTelemetryOptions.SectionName);

        section.Exists().Should().BeFalse();
        section.Bind(options);

        options.Logging.Enabled.Should().BeFalse();
        options.Tracing.SamplingProbability.Should().Be(1.0);
        options.Metrics.Meters.Should().BeEmpty();
    }
}
using Core.OpenApi.DependencyInjection;
using Core.OpenApi.Options;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

namespace Core.OpenApi.UnitTests;

public sealed class CoreOpenApiOptionsTests
{
    [Fact]
    public void ResolvedJsonEndpointRoute_DerivesFromDefaults()
    {
        var options = new CoreOpenApiOptions();

        options.ResolvedJsonEndpointRoute.Should().Be("/swagger/v1/swagger.json");
        options.ResolvedOriginalJsonRouteTemplate
            .Should()
            .Be("swagger-original/{documentName}/swagger.json");
    }

    [Fact]
    public void ResolvedJsonEndpointRoute_DerivesFromCustomPrefixAndDocumentName()
    {
        var options = new CoreOpenApiOptions
        {
            SwaggerRoutePrefix = "docs",
            DocumentName = "v2"
        };

        options.ResolvedJsonEndpointRoute.Should().Be("/docs/v2/swagger.json");
        options.ResolvedOriginalJsonRouteTemplate
            .Should()
            .Be("docs-original/{documentName}/swagger.json");
    }

    [Fact]
    public void ResolvedJsonEndpointRoute_KeepsExplicitRoute()
    {
        var options = new CoreOpenApiOptions
        {
            JsonEndpointRoute = "/openapi.json"
        };

        options.ResolvedJsonEndpointRoute.Should().Be("/openapi.json");
    }

    [Fact]
    public void Validate_Throws_WhenDocumentNameIsEmpty()
    {
        var options = new CoreOpenApiOptions { DocumentName = " " };

        var act = options.Validate;

        act.Should().Throw<ArgumentException>().WithParameterName(nameof(options.DocumentName));
    }

    [Fact]
    public void Validate_Throws_WhenDefinitionMissesErrorCode()
    {
        var options = new CoreOpenApiOptions();
        options.ProblemResponses.Definitions[0].ErrorCode = string.Empty;

        var act = options.Validate;

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void AddCoreOpenApi_RegistersSwaggerServices()
    {
        var services = new ServiceCollection();

        services.AddCoreOpenApi(options => options.Title = "Test API");

        services.Should().Contain(descriptor => descriptor.ServiceType == typeof(ISwaggerProvider));
        services.Should().Contain(
            descriptor => descriptor.ServiceType == typeof(IOptions<CoreOpenApiOptions>));
    }

    [Fact]
    public void AddCoreOpenApi_UsesEntryAssemblyName_WhenTitleIsMissing()
    {
        var services = new ServiceCollection();

        services.AddCoreOpenApi();

        using var provider = services.BuildServiceProvider();

        provider.GetRequiredService<IOptions<CoreOpenApiOptions>>().Value.Title
            .Should()
            .Be(typeof(CoreOpenApiOptionsTests).Assembly.GetName().Name);
    }
}

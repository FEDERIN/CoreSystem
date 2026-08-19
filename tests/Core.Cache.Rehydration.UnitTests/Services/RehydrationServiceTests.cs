using Core.Cache.Rehydration.Abstractions;
using Core.Cache.Rehydration.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Core.Cache.Rehydration.UnitTests.Services;

public sealed class RehydrationServiceTests
{
    [Fact]
    public async Task ExecuteCycleAsync_DoesNotRehydrate_WhenPrimaryIsHealthyInitially()
    {
        var healthCheckService = CreateHealthCheckService(
            HealthStatus.Healthy);

        var rehydrator = new Mock<ICacheRehydrator>();

        var service = CreateService(
            healthCheckService,
            rehydrator);

        await service.ExecuteCycleAsync(
            CancellationToken.None);

        rehydrator.Verify(
            x => x.RehydrateAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteCycleAsync_DoesNotRehydrate_WhenPrimaryIsUnhealthy()
    {
        var healthCheckService = CreateHealthCheckService(
            HealthStatus.Unhealthy);

        var rehydrator = new Mock<ICacheRehydrator>();

        var service = CreateService(
            healthCheckService,
            rehydrator);

        await service.ExecuteCycleAsync(
            CancellationToken.None);

        rehydrator.Verify(
            x => x.RehydrateAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteCycleAsync_Rehydrates_WhenPrimaryRecovers()
    {
        var healthCheckService = CreateHealthCheckService(
            HealthStatus.Unhealthy,
            HealthStatus.Healthy);

        var rehydrator = new Mock<ICacheRehydrator>();

        var service = CreateService(
            healthCheckService,
            rehydrator);

        await service.ExecuteCycleAsync(
            CancellationToken.None);

        await service.ExecuteCycleAsync(
            CancellationToken.None);

        rehydrator.Verify(
            x => x.RehydrateAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteCycleAsync_DoesNotRehydrateAgain_WhenPrimaryRemainsHealthyAfterRecovery()
    {
        var healthCheckService = CreateHealthCheckService(
            HealthStatus.Unhealthy,
            HealthStatus.Healthy,
            HealthStatus.Healthy);

        var rehydrator = new Mock<ICacheRehydrator>();

        var service = CreateService(
            healthCheckService,
            rehydrator);

        await service.ExecuteCycleAsync(
            CancellationToken.None);

        await service.ExecuteCycleAsync(
            CancellationToken.None);

        await service.ExecuteCycleAsync(
            CancellationToken.None);

        rehydrator.Verify(
            x => x.RehydrateAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteCycleAsync_DoesNotRehydrate_WhenNoPrimaryHealthChecksExist()
    {
        var healthCheckService = CreateHealthCheckService(
            includePrimary: false,
            HealthStatus.Unhealthy);

        var rehydrator = new Mock<ICacheRehydrator>();

        var service = CreateService(
            healthCheckService,
            rehydrator);

        await service.ExecuteCycleAsync(
            CancellationToken.None);

        rehydrator.Verify(
            x => x.RehydrateAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteCycleAsync_DoesNotRehydrate_WhenPrimaryCheckIsNotHealthy()
    {
        var healthCheckService = CreateHealthCheckService(
            HealthStatus.Degraded);

        var rehydrator = new Mock<ICacheRehydrator>();

        var service = CreateService(
            healthCheckService,
            rehydrator);

        await service.ExecuteCycleAsync(
            CancellationToken.None);

        rehydrator.Verify(
            x => x.RehydrateAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteCycleAsync_PropagatesCancellation()
    {
        using var cancellationTokenSource =
            new CancellationTokenSource();

        cancellationTokenSource.Cancel();

        var healthCheckService =
            CreateHealthCheckService(
                HealthStatus.Healthy);

        var rehydrator = new Mock<ICacheRehydrator>();

        var service = CreateService(
            healthCheckService,
            rehydrator);

        var exception = await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => service.ExecuteCycleAsync(
                cancellationTokenSource.Token));

        Assert.Equal(
            cancellationTokenSource.Token,
            exception.CancellationToken);

        rehydrator.Verify(
            x => x.RehydrateAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static RehydrationService CreateService(
        HealthCheckService healthCheckService,
        Mock<ICacheRehydrator> rehydrator)
    {
        return new RehydrationService(
            healthCheckService,
            rehydrator.Object,
            NullLogger<RehydrationService>.Instance);
    }

    private static HealthCheckService CreateHealthCheckService(
        params HealthStatus[] statuses)
    {
        return CreateHealthCheckService(
            includePrimary: true,
            statuses);
    }

    private static HealthCheckService CreateHealthCheckService(
        bool includePrimary,
        params HealthStatus[] statuses)
    {
        var services = new ServiceCollection();

        services.AddLogging();

        var healthChecks = services.AddHealthChecks();

        if (includePrimary)
        {
            healthChecks.AddCheck(
                "primary",
                new SequenceHealthCheck(statuses),
                tags: ["primary"]);
        }
        else
        {
            healthChecks.AddCheck(
                "database",
                new SequenceHealthCheck(statuses),
                tags: ["database"]);
        }

        var provider = services.BuildServiceProvider();

        return provider.GetRequiredService<HealthCheckService>();
    }

    private sealed class SequenceHealthCheck(
        params HealthStatus[] statuses) : IHealthCheck
    {
        private readonly Queue<HealthStatus> _statuses = new(statuses);

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var status = _statuses.Count > 0
                ? _statuses.Dequeue()
                : HealthStatus.Healthy;

            return Task.FromResult(
                new HealthCheckResult(status));
        }
    }
}
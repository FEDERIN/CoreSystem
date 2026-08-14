using Core.Cache.Rehydration.Abstractions;
using Core.Cache.Rehydration.Background;
using Core.Cache.Rehydration.Options;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Core.Cache.Rehydration.UnitTests.Background;

public sealed class RehydrationBackgroundServiceTests
{
    [Fact]
    public async Task StartAsync_ExecutesRehydrationCycle()
    {
        using var cancellationTokenSource =
            new CancellationTokenSource();

        var cycleExecuted =
            new TaskCompletionSource(
                TaskCreationOptions.RunContinuationsAsynchronously);

        var rehydrationService =
            new Mock<IRehydrationService>();

        rehydrationService
            .Setup(x => x.ExecuteCycleAsync(
                It.IsAny<CancellationToken>()))
            .Callback(() =>
            {
                cycleExecuted.SetResult();
            })
            .Returns(Task.CompletedTask);

        var service = CreateService(
            rehydrationService,
            TimeSpan.FromHours(1));

        var executionTask = service.StartAsync(
            cancellationTokenSource.Token);

        await cycleExecuted.Task;

        rehydrationService.Verify(
            x => x.ExecuteCycleAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);

        await cancellationTokenSource.CancelAsync();

        try
        {
            await executionTask;
        }
        catch (TaskCanceledException)
        {
            // BackgroundService is cancelled while waiting on Task.Delay.
        }
    }

    [Fact]
    public async Task StartAsync_DoesNotExecuteCycle_WhenCancellationIsAlreadyRequested()
    {
        using var cancellationTokenSource =
            new CancellationTokenSource();

        cancellationTokenSource.Cancel();

        var rehydrationService =
            new Mock<IRehydrationService>();

        var service = CreateService(
            rehydrationService,
            TimeSpan.Zero);

        await service.StartAsync(
            cancellationTokenSource.Token);

        rehydrationService.Verify(
            x => x.ExecuteCycleAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task StartAsync_ContinuesAfterRehydrationFailure()
    {
        using var cancellationTokenSource =
            new CancellationTokenSource();

        var secondExecution =
            new TaskCompletionSource(
                TaskCreationOptions.RunContinuationsAsynchronously);

        var rehydrationService =
            new Mock<IRehydrationService>();

        var executionCount = 0;

        rehydrationService
            .Setup(x => x.ExecuteCycleAsync(
                It.IsAny<CancellationToken>()))
            .Callback(() =>
            {
                executionCount++;

                if (executionCount == 2)
                {
                    secondExecution.SetResult();
                }
            })
            .ThrowsAsync(
                new InvalidOperationException("Test failure"));

        var service = CreateService(
            rehydrationService,
            TimeSpan.FromMilliseconds(10));

        var executionTask = service.StartAsync(
            cancellationTokenSource.Token);

        await secondExecution.Task;

        await cancellationTokenSource.CancelAsync();

        try
        {
            await executionTask;
        }
        catch (TaskCanceledException)
        {
            // Expected because Task.Delay observes the cancellation token.
        }

        Assert.Equal(2, executionCount);
    }

    [Fact]
    public async Task StartAsync_StopsWhenRehydrationIsCancelled()
    {
        using var cancellationTokenSource =
            new CancellationTokenSource();

        var rehydrationService =
            new Mock<IRehydrationService>();

        rehydrationService
            .Setup(x => x.ExecuteCycleAsync(
                It.IsAny<CancellationToken>()))
            .Callback(() =>
            {
                cancellationTokenSource.Cancel();
            })
            .ThrowsAsync(
                new OperationCanceledException());

        var service = CreateService(
            rehydrationService,
            TimeSpan.Zero);

        await service.StartAsync(
            cancellationTokenSource.Token);

        rehydrationService.Verify(
            x => x.ExecuteCycleAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private static RehydrationBackgroundService CreateService(
        Mock<IRehydrationService> rehydrationService,
        TimeSpan interval)
    {
        var options =
            Microsoft.Extensions.Options.Options.Create(
                new RehydrationOptions
                {
                    Enabled = true,
                    Interval = interval
                });

        return new RehydrationBackgroundService(
            rehydrationService.Object,
            NullLogger<RehydrationBackgroundService>.Instance,
            options);
    }
}
using Core.Cache.Options;
using Core.Cache.Services.Rehydration;
using Moq;

namespace Core.Cache.UnitTests.Services;

public sealed class RedisRehydrationBackgroundServiceTests
{
    private readonly CacheOptions _options = new()
    {
        RehydrationInterval = TimeSpan.FromSeconds(300)
    };

    [Fact]
    public async Task StartAsync_ShouldExecuteCycle()
    {
        // Arrange
        var executed = new TaskCompletionSource();

        var service = new Mock<IRedisRehydrationService>();

        service
            .Setup(x => x.ExecuteCycleAsync(It.IsAny<CancellationToken>()))
            .Returns<CancellationToken>(ct =>
            {
                executed.TrySetResult();

                return Task.CompletedTask;
            });

        var background = new RedisRehydrationBackgroundService(
            service.Object,
            _options);

        using var cts = new CancellationTokenSource();

        // Act
        await background.StartAsync(cts.Token);

        await executed.Task.WaitAsync(TimeSpan.FromSeconds(2), TestContext.Current.CancellationToken);

        await background.StopAsync(CancellationToken.None);

        // Assert
        service.Verify(
            x => x.ExecuteCycleAsync(It.IsAny<CancellationToken>()),
            Times.AtLeastOnce);
    }
}
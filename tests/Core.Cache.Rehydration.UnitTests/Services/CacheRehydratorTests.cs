using Core.Cache.Rehydration.Abstractions;
using Core.Cache.Rehydration.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace Core.Cache.Rehydration.UnitTests.Services;

public sealed class CacheRehydratorTests
{
    private readonly Mock<IRehydrationSource> _source;
    private readonly Mock<IRehydrationTarget> _target;
    private readonly Mock<ILogger<CacheRehydrator>> _logger;

    private readonly CacheRehydrator _sut;

    public CacheRehydratorTests()
    {
        _source = new Mock<IRehydrationSource>();
        _target = new Mock<IRehydrationTarget>();
        _logger = new Mock<ILogger<CacheRehydrator>>();

        _sut = new CacheRehydrator(
            _source.Object,
            _target.Object,
            _logger.Object);
    }

    [Fact]
    public async Task RehydrateAsync_StoresAllEntries()
    {
        var entries = new[]
        {
            CreateEntry("key-1"),
            CreateEntry("key-2"),
            CreateEntry("key-3")
        };

        _source
            .Setup(x => x.GetEntries())
            .Returns(entries);

        await _sut.RehydrateAsync(
            CancellationToken.None);

        foreach (var entry in entries)
        {
            _target.Verify(
                x => x.StoreAsync(
                    entry,
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }

    [Fact]
    public async Task RehydrateAsync_RemovesEntryAfterSuccessfulStorage()
    {
        var entry = CreateEntry("key-1");

        _source
            .Setup(x => x.GetEntries())
            .Returns([entry]);

        await _sut.RehydrateAsync(
            CancellationToken.None);

        _source.Verify(
            x => x.RemoveForRehydrationAsync(
                entry.Key,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task RehydrateAsync_DoesNotRemoveEntry_WhenStorageFails()
    {
        var entry = CreateEntry("key-1");

        _source
            .Setup(x => x.GetEntries())
            .Returns([entry]);

        _target
            .Setup(x => x.StoreAsync(
                entry,
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(
                new InvalidOperationException("Primary unavailable."));

        await _sut.RehydrateAsync(
            CancellationToken.None);

        _source.Verify(
            x => x.RemoveForRehydrationAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task RehydrateAsync_ContinuesWithNextEntry_WhenStorageFails()
    {
        var failedEntry = CreateEntry("key-1");
        var successfulEntry = CreateEntry("key-2");

        _source
            .Setup(x => x.GetEntries())
            .Returns(
            [
                failedEntry,
                successfulEntry
            ]);

        _target
            .Setup(x => x.StoreAsync(
                failedEntry,
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(
                new InvalidOperationException("Primary unavailable."));

        await _sut.RehydrateAsync(
            CancellationToken.None);

        _target.Verify(
            x => x.StoreAsync(
                failedEntry,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _target.Verify(
            x => x.StoreAsync(
                successfulEntry,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _source.Verify(
            x => x.RemoveForRehydrationAsync(
                successfulEntry.Key,
                It.IsAny<CancellationToken>()),
            Times.Once);

        _source.Verify(
            x => x.RemoveForRehydrationAsync(
                failedEntry.Key,
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task RehydrateAsync_ThrowsOperationCanceledException_WhenCancellationIsRequested()
    {
        var entry = CreateEntry("key-1");

        _source
            .Setup(x => x.GetEntries())
            .Returns([entry]);

        using var cancellationTokenSource =
            new CancellationTokenSource();

        cancellationTokenSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _sut.RehydrateAsync(
                cancellationTokenSource.Token));
    }

    [Fact]
    public async Task RehydrateAsync_PassesCancellationTokenToTarget()
    {
        var entry = CreateEntry("key-1");

        _source
            .Setup(x => x.GetEntries())
            .Returns([entry]);

        using var cancellationTokenSource =
            new CancellationTokenSource();

        var cancellationToken =
            cancellationTokenSource.Token;

        await _sut.RehydrateAsync(cancellationToken);

        _target.Verify(
            x => x.StoreAsync(
                entry,
                cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task RehydrateAsync_PassesCancellationTokenToSource()
    {
        var entry = CreateEntry("key-1");

        _source
            .Setup(x => x.GetEntries())
            .Returns([entry]);

        using var cancellationTokenSource =
            new CancellationTokenSource();

        var cancellationToken =
            cancellationTokenSource.Token;

        await _sut.RehydrateAsync(cancellationToken);

        _source.Verify(
            x => x.RemoveForRehydrationAsync(
                entry.Key,
                cancellationToken),
            Times.Once);
    }

    private static CacheRehydrationEntry CreateEntry(
        string key)
    {
        return new CacheRehydrationEntry
        {
            Key = key,
            Value = $"value-{key}"
        };
    }
}
using Core.Memory.Synchronization;
using FluentAssertions;

namespace Core.Memory.UnitTests.Synchronization;

public sealed class MemoryLockEntryTests
{
    [Fact]
    public void AddReference_ShouldIncrementReferenceCount()
    {
        // Arrange
        var entry = new MemoryLockEntry();

        // Act
        var first = entry.AddReference();
        var second = entry.AddReference();

        // Assert
        first.Should().Be(1);
        second.Should().Be(2);
    }


    [Fact]
    public void ReleaseReference_ShouldDecrementReferenceCount()
    {
        // Arrange
        var entry = new MemoryLockEntry();

        entry.AddReference();
        entry.AddReference();

        // Act
        var first = entry.ReleaseReference();
        var second = entry.ReleaseReference();

        // Assert
        first.Should().Be(1);
        second.Should().Be(0);
    }

    [Fact]
    public void Constructor_ShouldInitializeSemaphore()
    {
        // Arrange
        var entry = new MemoryLockEntry();

        // Assert
        entry.Semaphore.Should().NotBeNull();
        entry.Semaphore.CurrentCount.Should().Be(1);
    }
}

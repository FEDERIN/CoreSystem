namespace Core.Idempotency.UnitTests.DependencyInjection;

public class RedisRegistrationTests
{
    //[Fact]
    //public void AddCoreIdempotency_Should_Register_RedisProvider()
    //{
    //    // Arrange
    //    var services = new ServiceCollection();

    //    // Act
    //    services.AddCoreIdempotency(options =>
    //    {
    //        options.Enabled = true;
    //        options.Provider = IdempotencyProviderType.Redis;
    //        options.Redis.Configuration = conf =>
    //        {
    //            conf.EndPoints.Add("localhost:6379");
    //        };
    //    });

    //    var provider = services.BuildServiceProvider();

    //    // Assert
    //    provider.GetRequiredService<IKeyBuilder>();
    //    provider.GetRequiredService<IIdempotencyStorage>();
    //    provider.GetRequiredService<IConnectionMultiplexer>();
    //}

    //[Fact]
    //public void AddCoreIdempotency_Should_Throw_When_RedisConfiguration_Is_Missing()
    //{
    //    // Arrange
    //    var services = new ServiceCollection();

    //    // Act
    //    var action = () => services.AddCoreIdempotency(options =>
    //    {
    //        options.Enabled = true;
    //        options.Provider = IdempotencyProviderType.Redis;
    //    });

    //    // Assert
    //    action.Should()
    //          .Throw<InvalidOperationException>()
    //          .WithMessage(IdempotencyMessages.RedisConfigurationRequired);
    //}
}

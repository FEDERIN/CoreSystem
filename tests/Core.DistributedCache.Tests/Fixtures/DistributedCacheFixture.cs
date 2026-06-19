using StackExchange.Redis;

namespace Core.DistributedCache.Tests.Fixtures;

public class DistributedCacheFixture : IDisposable
{
    public IConnectionMultiplexer Redis { get; }

    public DistributedCacheFixture()
    {
        var options = ConfigurationOptions.Parse("localhost:6379");
        options.ConnectRetry = 3;
        options.SyncTimeout = 5000;

        Redis = ConnectionMultiplexer.Connect(options);


        if (!(Redis.GetDatabase().Ping().TotalMilliseconds > 0))
        {
            throw new Exception("Redis local no está respondiendo en el puerto 6379.");
        }
    }

    public void Dispose()
    {
        Redis.Dispose();

        GC.SuppressFinalize(this);
    }
}
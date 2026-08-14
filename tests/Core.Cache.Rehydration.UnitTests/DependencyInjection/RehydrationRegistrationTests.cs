using Core.Cache.Options;
using Core.Cache.Rehydration.Abstractions;
using Core.Cache.Rehydration.DependencyInjection;
using Core.Cache.Rehydration.Options;
using Core.Cache.Storage.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Core.Cache.Rehydration.UnitTests.DependencyInjection;

public sealed class RehydrationRegistrationTests
{
    [Fact]
    public void AddCoreCacheRehydration_RegistersAllRequiredServices()
    {
        var services = new ServiceCollection();

        services.AddSingleton(new CacheOptions
        {
            Enabled = true
        });

        services.AddSingleton<IExternalCacheStorage>(
            new FakeExternalCacheStorage());

        services.AddCoreCacheRehydration(options =>
        {
            options.Enabled = true;
            options.Interval = TimeSpan.FromSeconds(15);
        });

        var provider = services.BuildServiceProvider();

        Assert.Contains(
            services,
            descriptor =>
                descriptor.ServiceType == typeof(IRehydrationSource));

        Assert.Contains(
            services,
            descriptor =>
                descriptor.ServiceType == typeof(IRehydrationTarget));

        Assert.Contains(
            services,
            descriptor =>
                descriptor.ServiceType == typeof(ICacheRehydrator));

        Assert.Contains(
            services,
            descriptor =>
                descriptor.ServiceType == typeof(IRehydrationService));

        Assert.Contains(
            services,
            descriptor =>
                descriptor.ServiceType == typeof(IHostedService));
    }


    [Fact]
    public void AddCoreCacheRehydration_DoesNothing_WhenCacheIsDisabled()
    {
        var services = new ServiceCollection();

        services.AddSingleton(new CacheOptions
        {
            Enabled = false
        });

        services.AddCoreCacheRehydration(options =>
        {
            options.Enabled = true;
        });

        Assert.DoesNotContain(
            services,
            descriptor =>
                descriptor.ServiceType ==
                typeof(IRehydrationSource));

        Assert.DoesNotContain(
            services,
            descriptor =>
                descriptor.ServiceType ==
                typeof(IRehydrationTarget));

        Assert.DoesNotContain(
            services,
            descriptor =>
                descriptor.ServiceType ==
                typeof(ICacheRehydrator));

        Assert.DoesNotContain(
            services,
            descriptor =>
                descriptor.ServiceType ==
                typeof(IRehydrationService));

        Assert.DoesNotContain(
            services,
            descriptor =>
                descriptor.ServiceType ==
                typeof(IHostedService));
    }

    [Fact]
    public void AddCoreCacheRehydration_DoesNotRegisterServices_WhenRehydrationIsDisabled()
    {
        var services = new ServiceCollection();

        services.AddSingleton(new CacheOptions
        {
            Enabled = true
        });

        services.AddSingleton<IExternalCacheStorage>(
            new FakeExternalCacheStorage());

        services.AddCoreCacheRehydration(options =>
        {
            options.Enabled = false;
        });

        Assert.Contains(
            services,
            descriptor =>
                descriptor.ServiceType ==
                typeof(RehydrationOptions));

        Assert.DoesNotContain(
            services,
            descriptor =>
                descriptor.ServiceType ==
                typeof(IRehydrationSource));

        Assert.DoesNotContain(
            services,
            descriptor =>
                descriptor.ServiceType ==
                typeof(IRehydrationTarget));

        Assert.DoesNotContain(
            services,
            descriptor =>
                descriptor.ServiceType ==
                typeof(ICacheRehydrator));

        Assert.DoesNotContain(
            services,
            descriptor =>
                descriptor.ServiceType ==
                typeof(IRehydrationService));

        Assert.DoesNotContain(
            services,
            descriptor =>
                descriptor.ServiceType ==
                typeof(IHostedService));
    }

    [Fact]
    public void AddCoreCacheRehydration_Throws_WhenCacheOptionsImplementationInstanceIsInvalid()
    {
        var services = new ServiceCollection
        {
            new ServiceDescriptor(
                typeof(CacheOptions),
                new object())
        };

        var exception = Assert.Throws<InvalidOperationException>(
            () => services.AddCoreCacheRehydration(options =>
            {
                options.Enabled = true;
            }));

        Assert.Equal(
            RehydrationMessages.CacheRegistrationRequired,
            exception.Message);
    }

    [Fact]
    public void AddCoreCacheRehydration_Throws_WhenPrimaryIsNotRegistered()
    {
        var services = new ServiceCollection();

        services.AddSingleton(new CacheOptions
        {
            Enabled = true
        });

        var exception = Assert.Throws<InvalidOperationException>(
            () => services.AddCoreCacheRehydration(options =>
            {
                options.Enabled = true;
            }));

        Assert.Equal(
            RehydrationMessages.PrimaryRegistrationRequired,
            exception.Message);
    }
}

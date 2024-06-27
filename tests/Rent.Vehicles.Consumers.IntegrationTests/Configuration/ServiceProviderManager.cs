using Rent.Vehicles.Consumers.IntegrationTests.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace Rent.Vehicles.Consumers.IntegrationTests.Configuration;

public class ServiceProviderManager : IDisposable
{
    private static ServiceProviderManager? _serviceProviderManager;

    private readonly ServiceProvider _serviceProvider;

    private ServiceProviderManager(ServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public static ServiceProviderManager GetInstance(ITestOutputHelper output)
    {
        var serviceProvider = RegisterServices(output);

        _serviceProviderManager = new ServiceProviderManager(serviceProvider);

        return _serviceProviderManager;
    }

    private static ServiceProvider RegisterServices(ITestOutputHelper output)
    {
        var configuration = ConfigurationManager.GetInstance()
            .GetConfiguration();

        var services = new ServiceCollection();

        services.AddServicesTests(configuration, output)
                .AddSingleton(configuration);

        var serviceProvider = services.BuildServiceProvider();

        return serviceProvider;
    }

    public void Dispose()
    {
        if (_serviceProvider != null)
            _serviceProvider.Dispose();
    }

    public ServiceProvider GetServiceProvider()
    {
        return _serviceProvider;
    }
}
using Microsoft.Extensions.DependencyInjection;

using Rent.Vehicles.Consumers.IntegrationTests.Extensions.DependencyInjection;

namespace Rent.Vehicles.Consumers.IntegrationTests.Configuration;

public class ServiceProviderManager : IDisposable
{
    private static ServiceProviderManager? _serviceProviderManager;

    private readonly ServiceProvider _serviceProvider;

    private ServiceProviderManager(ServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }


    public void Dispose()
    {
        if (_serviceProvider != null)
        {
            _serviceProvider.Dispose();
        }
    }

    public static ServiceProviderManager GetInstance()
    {
        if (_serviceProviderManager != null)
        {
            return _serviceProviderManager;
        }

        var configuration = ConfigurationManager.GetInstance()
            .GetConfiguration();

        var services = new ServiceCollection();

        services.AddServicesTests(configuration)
            .AddSingleton(configuration);

        var serviceProvider = services.BuildServiceProvider();

        _serviceProviderManager = new ServiceProviderManager(serviceProvider);

        return _serviceProviderManager;
    }

    public ServiceProvider GetServiceProvider()
    {
        return _serviceProvider;
    }
}

using Microsoft.Extensions.DependencyInjection;

using Rent.Vehicles.Consumers.IntegrationTests.Configuration;

using Xunit.Abstractions;

namespace Rent.Vehicles.Consumers.IntegrationTests.ClassFixtures;

public class CommonFixture : IDisposable
{
    private ServiceProvider? _serviceProvider;

    public void Init(ITestOutputHelper output)
    {
        _serviceProvider = ServiceProviderManager
            .GetInstance(output)
            .GetServiceProvider();
    }

    public T GetRequiredService<T>() where T : class
    {
        return _serviceProvider!.GetRequiredService<T>();
    }

    public void Dispose()
    {
        _serviceProvider!.Dispose();
    }
}

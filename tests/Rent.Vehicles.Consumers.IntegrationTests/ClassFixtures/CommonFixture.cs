
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Rent.Vehicles.Consumers.IntegrationTests.Configuration;
using Rent.Vehicles.Entities.Contexts;
using Rent.Vehicles.Entities.Contexts.Interfaces;

using Xunit.Abstractions;

namespace Rent.Vehicles.Consumers.IntegrationTests.ClassFixtures;

public class CommonFixture : IAsyncLifetime
{
    private ServiceProvider? _serviceProvider;

    public T GetRequiredService<T>() where T : class
    {
        return _serviceProvider!.GetRequiredService<T>();
    }

    public async Task InitializeAsync()
    {
        if(_serviceProvider == null)
            _serviceProvider = ServiceProviderManager
                .GetInstance()
                .GetServiceProvider();

        var context = GetRequiredService<IDbContext>();

        await context.Database.EnsureDeletedAsync();

        await context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        var context = GetRequiredService<IDbContext>();

        await context.Database.EnsureDeletedAsync();

        await _serviceProvider!.DisposeAsync();
    }
}

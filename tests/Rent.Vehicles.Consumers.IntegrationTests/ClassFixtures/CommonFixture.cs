

using System.Data.Common;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Rent.Vehicles.Consumers.IntegrationTests.Configuration;
using Rent.Vehicles.Entities.Contexts;
using Rent.Vehicles.Entities.Contexts.Interfaces;
using Rent.Vehicles.Entities.Factories.Interfaces;

using Respawn;
using Respawn.Graph;

using Xunit.Abstractions;

namespace Rent.Vehicles.Consumers.IntegrationTests.ClassFixtures;

public class CommonFixture : IAsyncLifetime
{
    private ServiceProvider? _serviceProvider;

    public ServiceProvider ServiceProvider 
    {
        get => _serviceProvider ??= ServiceProviderManager
            .GetInstance()
            .GetServiceProvider();
    }

    public T GetRequiredService<T>() where T : class
    {
        return ServiceProvider.GetRequiredService<T>();
    }

    private Respawner _respawner = default!;

    private DbConnection _connection = default!;

    public async Task InitializeAsync()
    {
        // var contextFactory = GetRequiredService<IDbContextFactory>();

        // var context = await contextFactory.CreateDbContextAsync();
        var context = GetRequiredService<IDbContext>();

        await context.Database.MigrateAsync();

        _connection = context.Database.GetDbConnection();

        await _connection.OpenAsync();

		_respawner = await Respawner.CreateAsync(_connection,
			new RespawnerOptions
			{
				DbAdapter = DbAdapter.Postgres,
				SchemasToInclude = [ "vehicles", "events" ],
                TablesToIgnore = [ new Table("rentalPlanes") ],
				WithReseed = false
			}
		);

        // await context.Database.CloseConnectionAsync();

        // context.Dispose();
    }

    public async Task DisposeAsync()
    {
        var contextFactory = GetRequiredService<IDbContextFactory>();

        var context = await contextFactory.CreateDbContextAsync();

        await context.Database.EnsureDeletedAsync();

        context.Dispose();
    }

    public async Task ResetDatabaseAsync()
	{
        // var contextFactory = GetRequiredService<IDbContextFactory>();

        // var context = await contextFactory.CreateDbContextAsync();
        var context = GetRequiredService<IDbContext>();

        await context.Database.OpenConnectionAsync();

        context.ChangeTracker.Clear();

		await _respawner.ResetAsync(_connection);

        // await context.Database.CloseConnectionAsync();

        // context.Dispose();
	}
}

using System.Data.Entity.Infrastructure;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using MongoDB.Driver;

using Rent.Vehicles.Api;
using Rent.Vehicles.Consumers.IntegrationTests.Extensions.DependencyInjection;
using Rent.Vehicles.Entities;
using Rent.Vehicles.Entities.Factories.Interfaces;
using Rent.Vehicles.Services.Repositories.Interfaces;

using Respawn;
using Respawn.Graph;


namespace Rent.Vehicles.Consumers.IntegrationTests.ClassFixtures;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    protected override TestServer CreateServer(IWebHostBuilder builder)
    {
        builder
            .UseContentRoot(Directory.GetCurrentDirectory())
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                config.AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
            
                var configuration = context.Configuration;

                services.AddServicesTests(configuration);
            });

        return base.CreateServer(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
        builder
            .UseContentRoot(Directory.GetCurrentDirectory())
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                config.AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                    // Remova a configuração existente, se necessário
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IRepository<Vehicle>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
            
                var configuration = context.Configuration;

                services.AddServicesTests(configuration);
            });
        base.ConfigureWebHost(builder);
    }
    
    public async Task<T> SaveAsync<T>(T entity, CancellationToken cancellationToken = default) where T : Entity
    {
        var factory = Services.GetRequiredService<IDbContextFactory>();

        using var context = await factory.CreateDbContextAsync(cancellationToken);

        var repository = await context.Set<T>().AddAsync(entity, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

        

        return repository.Entity;
    }

    public async Task<TProjection> SaveProjectionAsync<TProjection>(TProjection entity, CancellationToken cancellationToken = default) where TProjection : Entity
    {
        var contextNoSql = Services.GetRequiredService<IMongoDatabase>();

        var mongoCollection = contextNoSql
            .GetCollection<TProjection>($"{typeof(TProjection).Name.ToLower()}s");

        await mongoCollection.InsertOneAsync(entity);

        return entity;
    }

    private Respawner _respawner = default!;

    public async Task InitializeAsync()
    {
        var factory = Services.GetRequiredService<IDbContextFactory>();

        using var context = await factory.CreateDbContextAsync();

        using var connection = context.Database.GetDbConnection();

        await context.Database.MigrateAsync();

        await connection.OpenAsync();

		_respawner = await Respawner.CreateAsync(connection,
			new RespawnerOptions
			{
				DbAdapter = DbAdapter.Postgres,
				SchemasToInclude = [ "vehicles", "events" ],
                TablesToIgnore = [ new Table("rentalPlanes") ],
				WithReseed = false
			}
		);
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        var factory = Services.GetRequiredService<IDbContextFactory>();

        using var context = await factory.CreateDbContextAsync();

        using var connection = context.Database.GetDbConnection();

        // await connection.OpenAsync();

        await context.Database.EnsureDeletedAsync();
    }

    public async Task ResetDatabaseAsync()
	{
        var factory = Services.GetRequiredService<IDbContextFactory>();

        using var context = await factory.CreateDbContextAsync();

        using var connection = context.Database.GetDbConnection();

        await connection.OpenAsync();

        context.ChangeTracker.Clear();

		await _respawner.ResetAsync(connection);
	}
}

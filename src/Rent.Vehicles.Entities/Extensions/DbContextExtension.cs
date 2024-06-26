using Microsoft.Extensions.DependencyInjection;

using Rent.Vehicles.Entities.Contexts.Interfaces;

using Microsoft.EntityFrameworkCore;
using Rent.Vehicles.Entities.Contexts;
using Rent.Vehicles.Entities.Factories;
using Rent.Vehicles.Entities.Factories.Interfaces;

namespace Rent.Vehicles.Entities.Extensions;

public static class DbContextExtension
{
    public static IServiceCollection AddRepository<TService, TImplementation>(this IServiceCollection services)
		where TService : class
		where TImplementation : class, TService, IRepository
	{
		return services.AddScoped<TService, TImplementation>()
			.AddScoped(service => (IRepository)service.GetRequiredService<TService>());
	}

    public static IServiceCollection AddDbContextDependencies<TInterface, TImplementation>(this IServiceCollection services,
        string connectionString)
        where TInterface : class, IDbContext
		where TImplementation : RentVehiclesContext, TInterface
    {
        return services.AddDbContextFactory<TImplementation>((service, options) =>
			{
				options.UseNpgsql(connectionString)
                    .UseSnakeCaseNamingConvention()
                    .UseLazyLoadingProxies(false);
			})
			.AddSingleton<IDbContextFactory>(service =>
			{
				var implementation = service.GetRequiredService<IDbContextFactory<TImplementation>>();

				return new DbContextFactory<TImplementation>(implementation);
			})
            .AddScoped<TInterface, TImplementation>(service =>
			{
				var factory = service.GetRequiredService<IDbContextFactory<TImplementation>>();
				var context = factory.CreateDbContext();

				return context;
			})
			.AddScoped<IUnitOfWorkerContext, TImplementation>(service =>
			{
				var factory = service.GetRequiredService<IDbContextFactory<TImplementation>>();
				var context = factory.CreateDbContext();

				return context;
			})
			.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}
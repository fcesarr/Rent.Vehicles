using Microsoft.Extensions.DependencyInjection;

using Rent.Vehicles.Entities.Contexts.Interfaces;

using Microsoft.EntityFrameworkCore;
using Rent.Vehicles.Entities.Contexts;
using Rent.Vehicles.Entities.Factories;
using Rent.Vehicles.Entities.Factories.Interfaces;

namespace Rent.Vehicles.Entities.Extensions;

public static class DbContextExtension
{
    public static IServiceCollection AddDbContext<TInterface, TImplementation>(this IServiceCollection services, string connectionString)
        where TInterface : class, IDbContext
		where TImplementation : VehiclesContext, TInterface
    {
        return services.AddDbContextFactory<TImplementation>((service, options) =>
			{
				options.UseNpgsql(connectionString)
                    .UseSnakeCaseNamingConvention()
                    .UseLazyLoadingProxies();
			})
			.AddSingleton<IDbContextFactory>(service =>
			{
				var implementation = service.GetRequiredService<IDbContextFactory<TImplementation>>();

				return new DbContextFactory<TImplementation>(implementation);
			});
    }
}
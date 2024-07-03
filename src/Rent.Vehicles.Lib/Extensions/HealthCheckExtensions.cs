using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Rent.Vehicles.Lib.Constants;
using Rent.Vehicles.Lib.HealthChecks;


namespace Rent.Vehicles.Lib.Extensions;

[ExcludeFromCodeCoverage]
public static class HealthCheckExtensions
{
	public static IServiceCollection AddHealthCheck(this IServiceCollection services,
		IConfiguration configuration)
	{
		var sqlConnectionString = configuration.GetConnectionString("Sql") ?? string.Empty;

		var noSqlConnectionString = configuration.GetConnectionString("NoSql") ?? string.Empty;

        var brokerConnectionString = configuration.GetConnectionString("Broker") ?? string.Empty;

		var healthCheckBuilder = services.AddHealthChecks()
			.AddCheck<VersionHealthCheck>("Infos", tags: new[] { HealthCheckTag.Ready, HealthCheckTag.Live })
			.AddNpgSql(sqlConnectionString, name: "Sql", tags: new[] { HealthCheckTag.Ready })
			.AddMongoDb(noSqlConnectionString, name: "NoSql", tags: new[] { HealthCheckTag.Ready })
            .AddRabbitMQ(brokerConnectionString, name: "Broker", tags: new[] { HealthCheckTag.Ready });

		return services;
	}
}

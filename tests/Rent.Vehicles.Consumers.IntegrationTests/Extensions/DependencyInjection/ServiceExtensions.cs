using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Services;
using Rent.Vehicles.Consumers.RabbitMQ.BackgroundServices;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Lib.Serializers;
using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Repositories.Interfaces;
using Rent.Vehicles.Services.Repositories;
using Npgsql;
using System.Text;
using System.Reflection;
using Rent.Vehicles.Services.Factories;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Producers.RabbitMQ;

namespace Rent.Vehicles.Consumers.IntegrationTests.Extensions.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class ServiceExtensions
{
    public static IServiceCollection AddServicesTests(this IServiceCollection services,
        IConfiguration configuration, ITestOutputHelper output)
            => services.AddLogging(output)
                .AddSingleton<IModel>(service => {
                    var factory = new ConnectionFactory { HostName = "localhost", Port = 5672, UserName = "admin", Password = "nimda" };
                    var connection = factory.CreateConnection();
                    return connection.CreateModel();
                })
                .AddSingleton<IRepository<Command>, Repository<Command>>(service =>
                {
                    var logger = service.GetRequiredService<ILogger<Repository<Command>>>();

                    var configuration = service.GetRequiredService<IConfiguration>();

                    var connectionString = configuration.GetConnectionString("Database") ?? string.Empty;

                    var connectionFactory = new ConnectionFactory<NpgsqlConnection>(connectionString);

                    var assembly = Assembly.GetExecutingAssembly();
                    var resourceNames = assembly.GetManifestResourceNames();
                    var sqlScripts = new Dictionary<string, string>();

                    string namespacePrefix = $"{assembly.GetName().Name}.Scripts.";

                    foreach (var resourceName in resourceNames)
                    {
                        if (resourceName.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
                        {
                            using (var stream = assembly.GetManifestResourceStream(resourceName))
                            using (var reader = new StreamReader(stream, Encoding.UTF8))
                            {
                                var sqlScript = reader.ReadToEnd();
                                sqlScripts.Add(resourceName.Replace(namespacePrefix, ""), sqlScript);
                            }
                        }
                    }

                    return new Repository<Command>(logger, sqlScripts, connectionFactory);
                })
                .AddSingleton<IDeleteService<Command>, Service<Command>>()
                .AddSingleton<ICreateService<Command>, Service<Command>>()
                .AddSingleton<IService<Command>, Service<Command>>()
                .AddSingleton<IPeriodicTimer>(service => {

                    var periodicTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(500));

                    return new Utils.PeriodicTimer(periodicTimer);
                })
                .AddSingleton<IPublisher, Publisher>()
                .AddSingleton<CreateVehiclesBackgroundService>()
                .AddSingleton<DeleteVehiclesBackgroundService>()
                .AddSingleton<ISerializer, MessagePackSerializer>();


    
    private static IServiceCollection AddLogging(this IServiceCollection services, ITestOutputHelper output)
            => services.AddLogging(configure => configure.AddConsole());
}
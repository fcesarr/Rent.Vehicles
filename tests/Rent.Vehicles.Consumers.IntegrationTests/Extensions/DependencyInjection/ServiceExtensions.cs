using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Services;
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
using Rent.Vehicles.Consumers.RabbitMQ.Commands.BackgroundServices;
using Rent.Vehicles.Consumers.RabbitMQ.Events.BackgroundServices;
using MongoDB.Driver;

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
                .AddSingleton<IMongoDatabase>(service => {
                    var configuration = service.GetRequiredService<IConfiguration>();

                    var connectionString = configuration.GetConnectionString("NoSql") ?? string.Empty;

                    var client = new MongoClient(connectionString);

                    return client.GetDatabase("rent");
                })
                .AddSingleton<IRepository<Vehicle>, MongoRepository<Vehicle>>()
                .AddSingleton<IRepository<Command>, EntityFrameworkRepository<Command>>()
                .AddSingleton<IDeleteService<Vehicle>, NoSqlService<Vehicle>>()
                .AddSingleton<ICreateService<Command>, SqlService<Command>>()
                .AddSingleton<ICreateService<Vehicle>, NoSqlService<Vehicle>>()
                .AddSingleton<IService<Command>, SqlService<Command>>()
                .AddTransient<IPeriodicTimer>(service => {

                    var periodicTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(500));

                    return new Utils.PeriodicTimer(periodicTimer);
                })
                .AddSingleton<IPublisher, Publisher>()
                .AddSingleton<ISerializer, MessagePackSerializer>()
                .AddSingleton<CreateVehiclesCommandBackgroundService>()
                .AddSingleton<DeleteVehiclesCommandBackgroundService>()
                .AddSingleton<CreateVehiclesForSpecificYearEventBackgroundService>()
                .AddSingleton<CreateVehiclesEventBackgroundService>()
                .AddSingleton<DeleteVehiclesEventBackgroundService>();


    
    private static IServiceCollection AddLogging(this IServiceCollection services, ITestOutputHelper output)
            => services.AddLogging(configure => configure.AddConsole());
}
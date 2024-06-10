using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Messages;
using Rent.Vehicles.Services;
using Rent.Vehicles.Consumers.RabbitMQ.BackgroundServices;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Lib.Serializers;

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
                .AddSingleton<ICreateService<CreateVehiclesCommand>, Services<CreateVehiclesCommand>>()
                .AddSingleton<CreateBackgroundService<CreateVehiclesCommand>>()
                .AddSingleton<IPeriodicTimer>(service => {

                    var periodicTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(500));

                    return new Rent.Vehicles.Consumers.Utils.PeriodicTimer(periodicTimer);
                })
                .AddSingleton<ISerializer, MessagePackSerializer>();


    
    private static IServiceCollection AddLogging(this IServiceCollection services, ITestOutputHelper output)
            => services.AddLogging(configure => configure.AddConsole());
}
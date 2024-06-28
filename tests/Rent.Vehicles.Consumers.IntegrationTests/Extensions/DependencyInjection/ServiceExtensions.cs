using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using MongoDB.Driver;

using RabbitMQ.Client;

using Rent.Vehicles.Consumers;
using Rent.Vehicles.Consumers.Commands.BackgroundServices;
using Rent.Vehicles.Consumers.Events.BackgroundServices;
using Rent.Vehicles.Consumers.Extensions;
using Rent.Vehicles.Consumers.Interfaces;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Entities;
using Rent.Vehicles.Entities.Contexts;
using Rent.Vehicles.Entities.Contexts.Interfaces;
using Rent.Vehicles.Entities.Extensions;
using Rent.Vehicles.Entities.Projections;
using Rent.Vehicles.Lib.Serializers;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Producers.RabbitMQ;
using Rent.Vehicles.Services;
using Rent.Vehicles.Services.DataServices;
using Rent.Vehicles.Services.DataServices.Interfaces;
using Rent.Vehicles.Services.Facades;
using Rent.Vehicles.Services.Facades.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Repositories;
using Rent.Vehicles.Services.Repositories.Interfaces;
using Rent.Vehicles.Services.Settings;
using Rent.Vehicles.Services.Validators;
using Rent.Vehicles.Services.Validators.Interfaces;

using Xunit.Abstractions;

namespace Rent.Vehicles.Consumers.IntegrationTests.Extensions.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class ServiceExtensions
{
    public static IServiceCollection AddServicesTests(this IServiceCollection services,
        IConfiguration configuration, ITestOutputHelper output)
            => services.AddLogging(output)
                .AddTransient<IConsumer>(service =>
                {
                    ConnectionFactory factory = new()
                    {
                        HostName = "localhost",
                        Port = 5672,
                        UserName = "admin",
                        Password = "nimda",
                        RequestedConnectionTimeout = TimeSpan.FromSeconds(30),
                        SocketReadTimeout = TimeSpan.FromSeconds(30),
                        SocketWriteTimeout = TimeSpan.FromSeconds(30)
                    };
                    var connection = factory.CreateConnection();
                    return new RabbitMQConsumer(connection.CreateModel());
                })
                .AddDbContextDependencies<IDbContext, RentVehiclesContext>(configuration.GetConnectionString("Sql") ??
                                                                        string.Empty)
                .AddTransient<IPeriodicTimer>(service =>
                {
                    PeriodicTimer periodicTimer = new(TimeSpan.FromMilliseconds(500));

                    return new Rent.Vehicles.Consumers.Utils.PeriodicTimer(periodicTimer);
                })
                // UserProjection
                .AddProjectionDomain<UserProjection, IUserProjectionDataService, UserProjectionDataService>()
                .AddScoped<IUserProjectionFacade, UserProjectionFacade>()
                // UserProjection
                // VehicleProjection
                .AddProjectionDomain<VehicleProjection>()
                .AddScoped<IVehicleProjectionFacade, VehicleProjectionFacade>()
                // VehicleProjection
                // VehiclesForSpecificYearProjection
                .AddProjectionDomain<VehiclesForSpecificYearProjection>()
                .AddScoped<IVehiclesForSpecificYearProjectionFacade, VehiclesForSpecificYearProjectionFacade>()
                // VehiclesForSpecificYearProjection
                // Event
                .AddDataDomain<Event, IValidator<Event>, Validator<Event>, IDataService<Event>, DataService<Event>>()
                .AddScoped<IEventFacade, EventFacade>()
                // Event
                // Command
                .AddDataDomain<Command, IValidator<Command>, Validator<Command> , ICommandDataService, CommandDataService>()
                .AddScoped<ICommandFacade,CommandFacade>()
                // Command
                // Vehicle
                .AddDataDomain<Vehicle, IVehicleValidator, VehicleValidator, IVehicleDataService, VehicleDataService>()
                .AddScoped<IVehicleFacade, VehicleFacade>()
                // Vehicle
                // User
                .AddDataDomain<User, IUserValidator, UserValidator, IUserDataService, UserDataService>()
                .AddScoped<IUserFacade, UserFacade>()
                // User
                // Rent
                .AddDataDomain<Rent.Vehicles.Entities.Rent, IRentValidator, RentValidator, IRentDataService, RentDataService>()
                .AddScoped<IRentFacade, RentFacade>()
                // Rent
                // RentPlane
                .AddDataDomain<RentalPlane, IValidator<RentalPlane>, Validator<RentalPlane>, IDataService<RentalPlane>, DataService<RentalPlane>>()
                // RentPlane
                .AddSingleton<IUploadService, FileUploadService>()
                .AddSingleton<ILicenseImageService, LicenseImageService>()
                .AddSingleton<IPublisher>(service => 
                {
                    ConnectionFactory factory = new()
                    {
                        HostName = "localhost",
                        Port = 5672,
                        UserName = "admin",
                        Password = "nimda",
                        RequestedConnectionTimeout = TimeSpan.FromSeconds(30),
                        SocketReadTimeout = TimeSpan.FromSeconds(30),
                        SocketWriteTimeout = TimeSpan.FromSeconds(30)
                    };
                    var connection = factory.CreateConnection();

                    var serializer = service.GetRequiredService<ISerializer>();
                    
                    return new RabbitMQPublisher(connection.CreateModel(), serializer);
                })
                .AddSingleton<Func<string, byte[], CancellationToken, Task>>(service => File.WriteAllBytesAsync)
                .AddSingleton<IMongoDatabase>(service =>
                {
                    var configuration = service.GetRequiredService<IConfiguration>();

                    var connectionString = configuration.GetConnectionString("NoSql") ?? string.Empty;

                    MongoClient client = new(connectionString);

                    return client.GetDatabase("rent");
                })
                .AddDefaultSerializer<MessagePackSerializer>()
                .AddSingleton<CreateRentCommandBackgroundService>()
                .AddSingleton<CreateUserCommandBackgroundService>()
                .AddSingleton<CreateVehiclesCommandBackgroundService>()
                .AddSingleton<DeleteVehiclesCommandBackgroundService>()
                .AddSingleton<UpdateRentCommandBackgroundService>()
                .AddSingleton<UpdateUserCommandBackgroundService>()
                .AddSingleton<UpdateUserLicenseImageCommandBackgroundService>()
                .AddSingleton<UpdateVehiclesCommandBackgroundService>()
                .AddSingleton<CreateRentEventBackgroundService>()
                .AddSingleton<CreateUserEventBackgroundService>()
                .AddSingleton<CreateUserProjectionEventBackgroundService>()
                .AddSingleton<CreateVehiclesEventBackgroundService>()
                .AddSingleton<CreateVehiclesForSpecificYearEventBackgroundService>()
                .AddSingleton<CreateVehiclesForSpecificYearProjectionEventBackgroundService>()
                .AddSingleton<CreateVehiclesProjectionEventBackgroundService>()
                .AddSingleton<DeleteVehiclesEventBackgroundService>()
                .AddSingleton<DeleteVehiclesProjectionEventBackgroundService>()
                .AddSingleton<EventBackgroundService>()
                .AddSingleton<UpdateRentEventBackgroundService>()
                .AddSingleton<UpdateUserEventBackgroundService>()
                .AddSingleton<UpdateUserLicenseImageEventBackgroundService>()
                .AddSingleton<UpdateUserProjectionEventBackgroundService>()
                .AddSingleton<UpdateVehiclesEventBackgroundService>()
                .AddSingleton<UpdateVehiclesProjectionEventBackgroundService>()
                .AddSingleton<UploadUserLicenseImageEventBackgroundService>();


    
    private static IServiceCollection AddLogging(this IServiceCollection services, ITestOutputHelper output)
            => services.AddLogging(configure => configure.AddConsole());
}

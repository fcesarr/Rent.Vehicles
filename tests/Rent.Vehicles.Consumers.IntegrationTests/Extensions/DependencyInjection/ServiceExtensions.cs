using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using MongoDB.Driver;

using RabbitMQ.Client;

using Rent.Vehicles.Consumers;
using Rent.Vehicles.Consumers.Commands.BackgroundServices;
using Rent.Vehicles.Consumers.Events.BackgroundServices;
using Rent.Vehicles.Services.Extensions;
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

using Serilog;

using Xunit.Abstractions;

namespace Rent.Vehicles.Consumers.IntegrationTests.Extensions.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class ServiceExtensions
{
    public static IServiceCollection AddServicesTests(this IServiceCollection services,
        IConfiguration configuration)
    {
        services = services.AddLogging(configuration)
            .AddTransient<IConsumer>(service =>
            {
                var connection = service.GetRequiredService<IConnection>();
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
            .AddProjectionDomain<UserProjection,
                IUserProjectionDataService,
                UserProjectionDataService,
                IUserProjectionFacade,
                UserProjectionFacade>()
            // UserProjection
            // VehicleProjection
            .AddProjectionDomain<VehicleProjection,
                IVehicleProjectionDataService,
                VehicleProjectionDataService,
                IVehicleProjectionFacade,
                VehicleProjectionFacade>()
            // VehicleProjection
            // VehiclesForSpecificYearProjection
            .AddProjectionDomain<VehiclesForSpecificYearProjection,
                IVehiclesForSpecificYearProjectionDataService,
                VehiclesForSpecificYearProjectionDataService,
                IVehiclesForSpecificYearProjectionFacade,
                VehiclesForSpecificYearProjectionFacade>()
            // VehiclesForSpecificYearProjection
            // RentProjection
            .AddProjectionDomain<RentProjection,
                IRentProjectionDataService,
                RentProjectionDataService,
                IRentProjectionFacade,
                RentProjectionFacade>()
            // RentProjection
            // EventProjection
            .AddProjectionDomain<EventProjection,
                IEventProjectionDataService,
                EventProjectionDataService,
                IEventProjectionFacade,
                EventProjectionFacade>()
            // EventProjection
            // Event
            .AddDataDomain<Event,
                IEventValidator,
                EventValidator,
                IEventDataService,
                EventDataService,
                IEventFacade,
                EventFacade>()
            // Event
            // Command
            .AddDataDomain<Command,
                ICommandValidator,
                CommandValidator,
                ICommandDataService,
                CommandDataService,
                ICommandFacade,
                CommandFacade>()
            // Command
            // Vehicle
            .AddDataDomain<Vehicle,
                IVehicleValidator,
                VehicleValidator,
                IVehicleDataService,
                VehicleDataService,
                IVehicleFacade,
                VehicleFacade>()
            // Vehicle
            // User
            .AddDataDomain<User,
                IUserValidator,
                UserValidator,
                IUserDataService,
                UserDataService,
                IUserFacade,
                UserFacade>()
            // User
            // Rent
            .AddDataDomain<Rent.Vehicles.Entities.Rent,
                IRentValidator,
                RentValidator,
                IRentDataService,
                RentDataService,
                IRentFacade,
                RentFacade>()
            // Rent
            // RentPlane
            .AddDataDomain<RentalPlane, IRentalPlaneValidator, RentalPlaneValidator, IRentalPlaneDataService, RentalPlaneDataService>()
            // RentPlane
            .AddSingleton<IStreamUploadService, StreamUploadService>()
            .AddSingleton<IUploadService>(services => services.GetRequiredService<IStreamUploadService>())
            .AddSingleton<IPublisher>(service => 
            {
                var connection = service.GetRequiredService<IConnection>();
                var serializer = service.GetRequiredService<ISerializer>();
                return new RabbitMQPublisher(connection.CreateModel(), serializer);
            })
            .AddSingleton<Func<string, byte[], CancellationToken, Task>>(service => File.WriteAllBytesAsync)
            .AddSingleton<IMongoDatabase>(service =>
            {
                var configuration = service.GetRequiredService<IConfiguration>();

                var connectionString = configuration.GetConnectionString("NoSql") ?? string.Empty;

                MongoClient client = new(connectionString);

                var databaseName = MongoUrl.Create(connectionString).DatabaseName;

                return client.GetDatabase(databaseName);
            })
            .AddSingleton<IConnection>(service => {
                var configuration = service.GetRequiredService<IConfiguration>();

                var connectionString = configuration.GetConnectionString("Broker") ?? string.Empty;

                var factory =  new ConnectionFactory 
                {
                    Uri = new Uri(connectionString)
                };

                return factory.CreateConnection();
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
            .AddSingleton<CreateRentProjectionEventBackgroundService>()
            .AddSingleton<CreateUserEventBackgroundService>()
            .AddSingleton<CreateUserProjectionEventBackgroundService>()
            .AddSingleton<CreateVehiclesEventBackgroundService>()
            .AddSingleton<CreateVehiclesForSpecificYearEventBackgroundService>()
            .AddSingleton<CreateVehiclesForSpecificYearProjectionEventBackgroundService>()
            .AddSingleton<CreateVehiclesProjectionEventBackgroundService>()
            .AddSingleton<DeleteVehiclesEventBackgroundService>()
            .AddSingleton<DeleteVehiclesProjectionEventBackgroundService>()
            .AddSingleton<EventBackgroundService>()
            .AddSingleton<EventProjectionEventBackgroundService>()
            .AddSingleton<UpdateRentEventBackgroundService>()
            .AddSingleton<UpdateRentProjectionEventBackgroundService>()
            .AddSingleton<UpdateUserEventBackgroundService>()
            .AddSingleton<UpdateUserLicenseImageEventBackgroundService>()
            .AddSingleton<UpdateUserProjectionEventBackgroundService>()
            .AddSingleton<UpdateVehiclesEventBackgroundService>()
            .AddSingleton<UpdateVehiclesProjectionEventBackgroundService>()
            .AddSingleton<UploadUserLicenseImageEventBackgroundService>();

        services.AddOptions<StreamUploadSetting>()
            .BindConfiguration(nameof(StreamUploadSetting))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
    
    private static IServiceCollection AddLogging(this IServiceCollection services,
        IConfiguration configuration)
            => services.AddLogging(configure => {
                var loggerConfiguration = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration);

                configure.AddSerilog(loggerConfiguration.CreateLogger(), true);
            });
}

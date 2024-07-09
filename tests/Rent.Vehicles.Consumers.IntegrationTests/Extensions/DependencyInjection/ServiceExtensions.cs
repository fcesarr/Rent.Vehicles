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
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Entities;
using Rent.Vehicles.Entities.Contexts;
using Rent.Vehicles.Entities.Contexts.Interfaces;
using Rent.Vehicles.Entities.Extensions;
using Rent.Vehicles.Entities.Projections;
using Rent.Vehicles.Lib.Serializers;
using Rent.Vehicles.Lib.Serializers.Interfaces;
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
using Rent.Vehicles.Lib.Extensions;

using Serilog;

using Xunit.Abstractions;
using Rent.Vehicles.Lib.Interfaces;
using Rent.Vehicles.Lib;
using Rent.Vehicles.Consumers.Settings;

namespace Rent.Vehicles.Consumers.IntegrationTests.Extensions.DependencyInjection;

[ExcludeFromCodeCoverage]
public static class ServiceExtensions
{
    public static IServiceCollection AddServicesTests(this IServiceCollection services,
        IConfiguration configuration)
    {
        services = services.AddLogging(configuration)
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
            .AddDataDomain<Entities.Event,
                IEventValidator,
                EventValidator,
                IEventDataService,
                EventDataService,
                IEventFacade,
                EventFacade>()
            // Event
            // Command
            .AddDataDomain<Entities.Command,
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
            .AddSingleton<Func<string, byte[], CancellationToken, Task>>(service => File.WriteAllBytesAsync)
            .AddSingleton<IMongoDatabase>(service =>
            {
                var configuration = service.GetRequiredService<IConfiguration>();

                var connectionString = configuration.GetConnectionString("NoSql") ?? string.Empty;

                MongoClient client = new(connectionString);

                var databaseName = MongoUrl.Create(connectionString).DatabaseName;

                return client.GetDatabase(databaseName);
            })
            .AddAmqpLiteBroker(configuration)
            .AddDefaultSerializer<MessagePackSerializer>()
            .AddHostedService<CreateRentCommandBackgroundService>()
            .AddHostedService<CreateUserCommandBackgroundService>()
            .AddHostedService<CreateVehiclesCommandBackgroundService>()
            .AddHostedService<DeleteVehiclesCommandBackgroundService>()
            .AddHostedService<UpdateRentCommandBackgroundService>()
            .AddHostedService<UpdateUserCommandBackgroundService>()
            .AddHostedService<UpdateUserLicenseImageCommandBackgroundService>()
            .AddHostedService<UpdateVehiclesCommandBackgroundService>()
            .AddHostedService<CreateRentEventBackgroundService>()
            .AddHostedService<CreateRentProjectionEventBackgroundService>()
            .AddHostedService<CreateUserEventBackgroundService>()
            .AddHostedService<CreateUserProjectionEventBackgroundService>()
            .AddHostedService<CreateVehiclesEventBackgroundService>()
            .AddHostedService<CreateVehiclesForSpecificYearEventBackgroundService>()
            .AddHostedService<CreateVehiclesForSpecificYearProjectionEventBackgroundService>()
            .AddHostedService<CreateVehiclesProjectionEventBackgroundService>()
            .AddHostedService<DeleteVehiclesEventBackgroundService>()
            .AddHostedService<DeleteVehiclesProjectionEventBackgroundService>()
            .AddHostedService<EventBackgroundService>()
            .AddHostedService<EventProjectionEventBackgroundService>()
            .AddHostedService<UpdateRentEventBackgroundService>()
            .AddHostedService<UpdateRentProjectionEventBackgroundService>()
            .AddHostedService<UpdateUserEventBackgroundService>()
            .AddHostedService<UpdateUserLicenseImageEventBackgroundService>()
            .AddHostedService<UpdateUserProjectionEventBackgroundService>()
            .AddHostedService<UpdateVehiclesEventBackgroundService>()
            .AddHostedService<UpdateVehiclesProjectionEventBackgroundService>()
            .AddHostedService<UploadUserLicenseImageEventBackgroundService>();

        services.AddOptions<StreamUploadSetting>()
            .BindConfiguration(nameof(StreamUploadSetting))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services.AddOptions<ConsumerSetting>()
            .BindConfiguration(nameof(ConsumerSetting))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
    
    public static IServiceCollection AddLogging(this IServiceCollection services,
        IConfiguration configuration)
            => services.AddLogging(configure => {
                var loggerConfiguration = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration);
                configure.AddConsole();
                //configure.AddSerilog(loggerConfiguration.CreateLogger(), true);
            });
}

using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Lib.Serializers;
using Rent.Vehicles.Services.Interfaces;
using RabbitMQ.Client;
using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Repositories.Interfaces;
using Rent.Vehicles.Services.Repositories;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Producers.RabbitMQ;
using Rent.Vehicles.Consumers.RabbitMQ.Commands.BackgroundServices;
using Rent.Vehicles.Consumers.RabbitMQ.Events.BackgroundServices;
using MongoDB.Driver;
using Rent.Vehicles.Entities.Contexts;
using Rent.Vehicles.Entities.Extensions;
using Rent.Vehicles.Entities.Contexts.Interfaces;
using Rent.Vehicles.Services.Validators.Interfaces;
using Rent.Vehicles.Services.Validators;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Services;
using Rent.Vehicles.Entities.Projections;
using Rent.Vehicles.Consumers.Extensions;
using Rent.Vehicles.Services.Settings;
using Rent.Vehicles.Services.Facades.Interfaces;
using Rent.Vehicles.Services.DataServices.Interfaces;
using Rent.Vehicles.Services.DataServices;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<LicenseImageSetting>(builder.Configuration.GetSection("LicenseImageSetting"));

builder.Services
    .AddSingleton<Func<string, byte[], CancellationToken, Task>>(service => File.WriteAllBytesAsync)
    .AddSingleton<IModel>(service => {
        var factory = new ConnectionFactory { HostName = "localhost", Port = 5672, UserName = "admin", Password = "nimda" };
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        channel.ExchangeDeclare(exchange: typeof(CreateUserSuccessEvent).Name,
            type: "fanout",
            durable: true,
            autoDelete: false);

        channel.ExchangeDeclare(exchange: typeof(CreateVehiclesSuccessEvent).Name,
            type: "fanout",
            durable: true,
            autoDelete: false);

        return channel;
    })
    .AddDbContextDependencies<IDbContext, RentVehiclesContext>(builder.Configuration.GetConnectionString("Sql") ?? string.Empty)
    .AddSingleton<IMongoDatabase>(service => {
        var configuration = service.GetRequiredService<IConfiguration>();

        var connectionString = configuration.GetConnectionString("NoSql") ?? string.Empty;

        var client = new MongoClient(connectionString);

        return client.GetDatabase("rent");
    })
    .AddTransient<IPeriodicTimer>(service => {

        var periodicTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(500));

        return new Rent.Vehicles.Consumers.Utils.PeriodicTimer(periodicTimer);
    })
    .AddDataDomain<Command>()
    .AddProjectionDomain<VehicleProjection, IVehicleProjectionDataService, VehicleProjectionDataService>()
    .AddProjectionDomain<VehiclesForSpecificYearProjection>()
    .AddProjectionDomain<Rent.Vehicles.Entities.Event>()
    .AddDataDomain<Vehicle, IVehicleValidator, VehicleValidator, IVehicleDataService, VehicleDataService>()
    .AddDataDomain<User, IUserValidator, UserValidator, IUserDataService, UserDataService>()
    .AddDefaultSerializer<MessagePackSerializer>()
    .AddSingleton<IPublisher, Publisher>()
    .AddDataDomain<User, IUserValidator, UserValidator, IUserDataService, UserDataService>()
    .AddSingleton<IBase64StringValidator, Base64StringValidator>()
    .AddSingleton<IUserFacade, UserFacade>()
    .AddSingleton<IUploadService, FileUploadService>()
    .AddSingleton<ILicenseImageService, LicenseImageService>()
    .AddHostedService<CreateVehiclesCommandSqlBackgroundService>()
    .AddHostedService<CreateVehiclesEventSqlBackgroundService>()
    .AddHostedService<CreateVehiclesForSpecificYearEventNoSqlBackgroundService>()
    .AddHostedService<CreateVehiclesSuccessEventNoSqlBackgroundService>()
    .AddHostedService<CreateVehiclesSuccessEventSpecificYearBackgroundService>()
    .AddHostedService<DeleteVehiclesCommandSqlBackgroundService>()
    .AddHostedService<DeleteVehiclesEventSqlBackgroundService>()
    .AddHostedService<DeleteVehiclesSuccessEventNoSqlBackgroundService>()
    .AddHostedService<UpdateVehiclesCommandSqlBackgroundService>()
    .AddHostedService<UpdateVehiclesEventSqlBackgroundService>()
    .AddHostedService<UpdateVehiclesSuccessEventNoSqlBackgroundService>()
    .AddHostedService<EventBackgroundService>()
    .AddHostedService<CreateUserCommandSqlBackgroundService>()
    .AddHostedService<CreateUserEventSqlBackgroundService>()
    .AddHostedService<CreateUserSuccessEventUploadLicenseImageBackgroundService>();

var host = builder.Build();
host.Run();

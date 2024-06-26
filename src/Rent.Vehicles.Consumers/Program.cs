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
using Rent.Vehicles.Consumers.Commands.BackgroundServices;
using Rent.Vehicles.Consumers.Events.BackgroundServices;
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
using Rent.Vehicles.Consumers.Interfaces;
using Rent.Vehicles.Consumers;
using Rent.Vehicles.Services.Facades;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<LicenseImageSetting>(builder.Configuration.GetSection("LicenseImageSetting"));

builder.Services
    .AddSingleton<Func<string, byte[], CancellationToken, Task>>(service => File.WriteAllBytesAsync)
    .AddTransient<IModel>(service => {
        var factory = new ConnectionFactory { 
            HostName = "localhost",
            Port = 5672,
            UserName = "admin",
            Password = "nimda",
            RequestedConnectionTimeout = TimeSpan.FromSeconds(30),
            SocketReadTimeout = TimeSpan.FromSeconds(30),
            SocketWriteTimeout = TimeSpan.FromSeconds(30)    
        };
        var connection = factory.CreateConnection();
        return connection.CreateModel();
    })
    .AddTransient<IConsumer>(service => {
        var factory = new ConnectionFactory { 
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
    .AddScoped<IValidator<Command>, Validator<Command>>()
    .AddRepository<IRepository<Command>, EntityFrameworkRepository<Command>>()
    .AddScoped<ICommandDataService, CommandDataService>()

    .AddProjectionDomain<VehicleProjection, IVehicleProjectionDataService, VehicleProjectionDataService>()
    .AddProjectionDomain<VehiclesForSpecificYearProjection>()
    .AddDataDomain<Rent.Vehicles.Entities.Event>()
    .AddDataDomain<Vehicle, IVehicleValidator, VehicleValidator, IVehicleDataService, VehicleDataService>()
    .AddDataDomain<User, IUserValidator, UserValidator, IUserDataService, UserDataService>()
    .AddDefaultSerializer<MessagePackSerializer>()
    .AddDataDomain<User, IUserValidator, UserValidator, IUserDataService, UserDataService>()
    .AddScoped<IUserFacade, UserFacade>()
    .AddScoped<IRentValidator, RentValidator>()
    .AddScoped<IRepository<Rent.Vehicles.Entities.Rent>, EntityFrameworkRepository<Rent.Vehicles.Entities.Rent>>()
    .AddScoped<IRentDataService, RentDataService>()
    .AddScoped<IValidator<RentalPlane>, Validator<RentalPlane>>()
    .AddScoped<IRepository<RentalPlane>, EntityFrameworkRepository<RentalPlane>>()
    .AddScoped<IDataService<RentalPlane>, DataService<RentalPlane>>()
    .AddScoped<IVehicleDataService, VehicleDataService>()
    .AddScoped<IRentFacade, RentFacade>()
    .AddSingleton<IBase64StringValidator, Base64StringValidator>()
    .AddSingleton<IUploadService, FileUploadService>()
    .AddSingleton<ILicenseImageService, LicenseImageService>()
    .AddSingleton<ISerializer, MessagePackSerializer>()
    .AddSingleton<IPublisher, Publisher>()
    .AddHostedService<CreateVehiclesCommandSqlBackgroundService>()
    .AddHostedService<DeleteVehiclesCommandSqlBackgroundService>()
    .AddHostedService<UpdateVehiclesCommandSqlBackgroundService>()
    .AddHostedService<CreateUserCommandSqlBackgroundService>()
    .AddHostedService<CreateRentCommandSqlBackgroundService>()
    .AddHostedService<UpdateRentCommandSqlBackgroundService>()
    .AddHostedService<CreateUserEventBackgroundService>()
    .AddHostedService<CreateUserProjectionEventBackgroundService>()
    .AddHostedService<CreateVehiclesEventBackgroundService>()
    .AddHostedService<CreateVehiclesForSpecificYearEventBackgroundService>()
    .AddHostedService<CreateVehiclesForSpecificYearProjectionEventBackgroundService>()
    .AddHostedService<CreateVehiclesProjectionEventBackgroundService>()
    .AddHostedService<DeleteVehiclesEventBackgroundService>()
    .AddHostedService<DeleteVehiclesProjectionEventBackgroundService>()
    .AddHostedService<EventBackgroundService>()
    .AddHostedService<UpdateVehiclesEventBackgroundService>()
    .AddHostedService<UpdateVehiclesProjectionEventBackgroundService>()
    .AddHostedService<UploadUserLicenseImageEventBackgroundService>()
    .AddHostedService<CreateRentEventBackgroundService>()
    .AddHostedService<UpdateRentEventBackgroundService>();

var host = builder.Build();
host.Run();

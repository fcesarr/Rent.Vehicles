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

var builder = Host.CreateApplicationBuilder(args);


builder.Services
    .AddSingleton<IModel>(service => {
        var factory = new ConnectionFactory { HostName = "localhost", Port = 5672, UserName = "admin", Password = "nimda" };
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

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
    .AddSingleton<IValidator<Command>, Validator<Command>>()
    .AddSingleton<ISqlRepository<Command>, EntityFrameworkRepository<Command>>()
    .AddSingleton<ISqlService<Command>, Service<Command>>()
    .AddSingleton<IValidator<Vehicle>, Validator<Vehicle>>()
    .AddSingleton<ISqlRepository<Vehicle>, EntityFrameworkRepository<Vehicle>>()
    .AddSingleton<ISqlVehiclesService, VehiclesService>(service => {
        var logger = service.GetRequiredService<ILogger<Service<Vehicle>>>();

        var validator = service.GetRequiredService<IValidator<Vehicle>>();
        
        var repository = service.GetRequiredService<ISqlRepository<Vehicle>>();

        return new VehiclesService(logger, validator, repository);
    })
    .AddSingleton<INoSqlRepository<Vehicle>, MongoRepository<Vehicle>>()
    .AddSingleton<INoSqlVehiclesService, VehiclesService>(service => {
        var logger = service.GetRequiredService<ILogger<Service<Vehicle>>>();

        var validator = service.GetRequiredService<IValidator<Vehicle>>();
        
        var repository = service.GetRequiredService<INoSqlRepository<Vehicle>>();

        return new VehiclesService(logger, validator, repository);
    }
    )
    .AddSingleton<IValidator<VehiclesForSpecificYear>, Validator<VehiclesForSpecificYear>>()
    .AddSingleton<INoSqlRepository<VehiclesForSpecificYear>, MongoRepository<VehiclesForSpecificYear>>()
    .AddSingleton<INoSqlService<VehiclesForSpecificYear>, Service<VehiclesForSpecificYear>>()
    .AddSingleton<IPublisher, Publisher>()
    .AddSingleton<ISerializer, MessagePackSerializer>()
    .AddSingleton<IValidator<Rent.Vehicles.Entities.Event>, Validator<Rent.Vehicles.Entities.Event>>()
    .AddSingleton<INoSqlRepository<Rent.Vehicles.Entities.Event>, MongoRepository<Rent.Vehicles.Entities.Event>>()
    .AddSingleton<INoSqlService<Rent.Vehicles.Entities.Event>, Service<Rent.Vehicles.Entities.Event>>()
    .AddHostedService<CreateVehiclesCommandSqlBackgroundService>()
    .AddHostedService<CreateVehiclesEventSqlBackgroundService>(service => {
        var logger = service.GetRequiredService<ILogger<CreateVehiclesEventSqlBackgroundService>>();

        var channel = service.GetRequiredService<IModel>();

        var periodicTimer = service.GetRequiredService<IPeriodicTimer>();

        var serializer = service.GetRequiredService<ISerializer>();

        var publisher = service.GetRequiredService<IPublisher>();

        var sqlService = service.GetRequiredService<ISqlVehiclesService>();

        return new(logger, channel, periodicTimer, serializer, publisher, (IVehiclesService)sqlService);
    })
    .AddHostedService<CreateVehiclesForSpecificYearEventNoSqlBackgroundService>()
    .AddHostedService<CreateVehiclesSuccessEventNoSqlBackgroundService>(service => {
        var logger = service.GetRequiredService<ILogger<CreateVehiclesSuccessEventNoSqlBackgroundService>>();

        var channel = service.GetRequiredService<IModel>();

        var periodicTimer = service.GetRequiredService<IPeriodicTimer>();

        var serializer = service.GetRequiredService<ISerializer>();

        var publisher = service.GetRequiredService<IPublisher>();

        var sqlService = service.GetRequiredService<INoSqlVehiclesService>();

        return new(logger, channel, periodicTimer, serializer, publisher, (IVehiclesService)sqlService);
    })
    .AddHostedService<CreateVehiclesSuccessEventSpecificYearBackgroundService>()
    .AddHostedService<DeleteVehiclesCommandSqlBackgroundService>()
    .AddHostedService<DeleteVehiclesEventSqlBackgroundService>(service => {
        var logger = service.GetRequiredService<ILogger<DeleteVehiclesEventSqlBackgroundService>>();

        var channel = service.GetRequiredService<IModel>();

        var periodicTimer = service.GetRequiredService<IPeriodicTimer>();

        var serializer = service.GetRequiredService<ISerializer>();

        var publisher = service.GetRequiredService<IPublisher>();

        var sqlService = service.GetRequiredService<ISqlVehiclesService>();

        return new(logger, channel, periodicTimer, serializer, publisher, (IVehiclesService)sqlService);
    })
    .AddHostedService<DeleteVehiclesSuccessEventNoSqlBackgroundService>(service => {
        var logger = service.GetRequiredService<ILogger<DeleteVehiclesSuccessEventNoSqlBackgroundService>>();

        var channel = service.GetRequiredService<IModel>();

        var periodicTimer = service.GetRequiredService<IPeriodicTimer>();

        var serializer = service.GetRequiredService<ISerializer>();

        var publisher = service.GetRequiredService<IPublisher>();

        var sqlService = service.GetRequiredService<INoSqlVehiclesService>();

        return new(logger, channel, periodicTimer, serializer, publisher, (IVehiclesService)sqlService);
    })
    .AddHostedService<UpdateVehiclesCommandSqlBackgroundService>()
    .AddHostedService<UpdateVehiclesEventSqlBackgroundService>()
    .AddHostedService<UpdateVehiclesSuccessEventNoSqlBackgroundService>()
    .AddHostedService<EventBackgroundService>();

var host = builder.Build();
host.Run();

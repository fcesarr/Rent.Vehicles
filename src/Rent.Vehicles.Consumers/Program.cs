using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Lib.Serializers;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services;
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
using System.ComponentModel.DataAnnotations;
using Rent.Vehicles.Services.Validators;
using Rent.Vehicles.Consumers.Extensions;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson;
using Rent.Vehicles.Messages.Events;

var builder = Host.CreateApplicationBuilder(args);

// BsonClassMap.RegisterClassMap<Vehicle>(
//     map =>
//     {
//         map.AutoMap();
//         map.MapIdProperty("Id");
//     });

builder.Services
    .AddSingleton<IModel>(service => {
        var factory = new ConnectionFactory { HostName = "localhost", Port = 5672, UserName = "admin", Password = "nimda" };
        var connection = factory.CreateConnection();
        var channel = connection.CreateModel();

        channel.ExchangeDeclare(exchange: typeof(CreateVehiclesEvent).Name,
            type: "fanout",
            durable: true,
            autoDelete: false);

        channel.ExchangeDeclare(exchange: typeof(DeleteVehiclesEvent).Name,
            type: "fanout",
            durable: true,
            autoDelete: false);

        channel.ExchangeDeclare(exchange: typeof(UpdateVehiclesEvent).Name,
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
    .AddSingleton<ISqlService<Command>, SqlService<Command>>()
    .AddSingleton<IValidator<Vehicle>, Validator<Vehicle>>()
    .AddSingleton<ISqlRepository<Vehicle>, EntityFrameworkRepository<Vehicle>>()
    .AddSingleton<ISqlService<Vehicle>, SqlService<Vehicle>>()
    .AddSingleton<INoSqlRepository<Vehicle>, MongoRepository<Vehicle>>()
    .AddSingleton<INoSqlService<Vehicle>, NoSqlService<Vehicle>>()
    .AddSingleton<IValidator<VehiclesForSpecificYear>, Validator<VehiclesForSpecificYear>>()
    .AddSingleton<INoSqlRepository<VehiclesForSpecificYear>, MongoRepository<VehiclesForSpecificYear>>()
    .AddSingleton<INoSqlService<VehiclesForSpecificYear>, NoSqlService<VehiclesForSpecificYear>>()
    .AddSingleton<IPublisher, Publisher>()
    .AddSingleton<ISerializer, MessagePackSerializer>()
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
    .AddHostedService<UpdateVehiclesSuccessEventNoSqlBackgroundService>();

var host = builder.Build();
host.Run();

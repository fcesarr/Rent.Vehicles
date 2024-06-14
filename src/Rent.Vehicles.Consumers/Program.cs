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
        return connection.CreateModel();
    })
    .AddDbContextDependencies<IDbContext, RentVehiclesContext>(builder.Configuration.GetConnectionString("Sql") ?? string.Empty)
    .AddSingleton<IMongoDatabase>(service => {
        var configuration = service.GetRequiredService<IConfiguration>();

        var connectionString = configuration.GetConnectionString("NoSql") ?? string.Empty;

        var client = new MongoClient(connectionString);

        return client.GetDatabase("rent");
    })
    .AddCreateSqlService<Command>()
    .AddCreateBothService<Event>()
    .AddSingleton<IValidator<VehiclesForSpecificYear>, Validator<VehiclesForSpecificYear>>()
    .AddSingleton<IValidator<Vehicle>, VehicleValidator>()
    .AddSingleton<IRepository<Vehicle>, MongoRepository<Vehicle>>()
    .AddSingleton<IRepository<VehiclesForSpecificYear>, MongoRepository<VehiclesForSpecificYear>>()
    .AddSingleton<IDeleteService<Vehicle>, NoSqlService<Vehicle>>()
    .AddSingleton<ICreateService<Vehicle>, NoSqlService<Vehicle>>()
    .AddSingleton<IUpdateService<Vehicle>, NoSqlService<Vehicle>>()
    .AddSingleton<ICreateService<VehiclesForSpecificYear>, NoSqlService<VehiclesForSpecificYear>>()
    .AddTransient<IPeriodicTimer>(service => {

        var periodicTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(500));

        return new Rent.Vehicles.Consumers.Utils.PeriodicTimer(periodicTimer);
    })
    .AddSingleton<IPublisher, Publisher>()
    .AddSingleton<ISerializer, MessagePackSerializer>()
    .AddHostedService<CreateVehiclesCommandBackgroundService>()
    .AddHostedService<DeleteVehiclesCommandBackgroundService>()
    .AddHostedService<CreateVehiclesForSpecificYearEventBackgroundService>()
    .AddHostedService<CreateVehiclesEventBackgroundService>()
    .AddHostedService<DeleteVehiclesEventBackgroundService>()
    .AddHostedService<EnqueueVehiclesForSpecificYearEventBackgroundService>()
    .AddHostedService<UpdateVehiclesCommandBackgroundService>()
    .AddSingleton<IVehicleService, VehicleService>()
    .AddHostedService<UpdateVehiclesEventBackgroundService>();

var host = builder.Build();
host.Run();

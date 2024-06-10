using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Lib.Serializers;
using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services;
using Rent.Vehicles.Consumers.RabbitMQ.BackgroundServices;
using RabbitMQ.Client;
using Rent.Vehicles.Entities;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IModel>(service => {
    var factory = new ConnectionFactory { HostName = "localhost", Port = 5672, UserName = "user", Password = "password" };
    var connection = factory.CreateConnection();
    return connection.CreateModel();
});

builder.Services.AddSingleton<ICreateService<Command>, Services<Command>>();

builder.Services.AddHostedService<CreateBackgroundService<CreateVehiclesCommand, Command>>();

builder.Services.AddSingleton<IPeriodicTimer>(service => {

    var periodicTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(500));

    return new Rent.Vehicles.Consumers.Utils.PeriodicTimer(periodicTimer);
});

builder.Services.AddSingleton<ISerializer, MessagePackSerializer>();

var host = builder.Build();
host.Run();

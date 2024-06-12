using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Lib.Serializers;
using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Factories;
using Rent.Vehicles.Services;
using RabbitMQ.Client;
using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Repositories.Interfaces;
using Rent.Vehicles.Services.Repositories;
using Npgsql;
using System.Reflection;
using System.Text;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Producers.RabbitMQ;
using Rent.Vehicles.Consumers.RabbitMQ.Commands.BackgroundServices;
using Rent.Vehicles.Consumers.RabbitMQ.Events.BackgroundServices;
using MongoDB.Driver;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IModel>(service => {
        var factory = new ConnectionFactory { HostName = "localhost", Port = 5672, UserName = "admin", Password = "nimda" };
        var connection = factory.CreateConnection();
        return connection.CreateModel();
    })
    .AddSingleton<IMongoDatabase>(service => {
        var configuration = service.GetRequiredService<IConfiguration>();

        var connectionString = configuration.GetConnectionString("NoSql") ?? string.Empty;

        var client = new MongoClient(connectionString);

        return client.GetDatabase("rent");
    })
    .AddSingleton<IMongoRepository<Vehicle>, MongoRepository<Vehicle>>()
    .AddSingleton<IRepository<Command>, Repository<Command>>(service =>
    {
        var logger = service.GetRequiredService<ILogger<Repository<Command>>>();

        var configuration = service.GetRequiredService<IConfiguration>();

        var connectionString = configuration.GetConnectionString("Sql") ?? string.Empty;

        var connectionFactory = new ConnectionFactory<NpgsqlConnection>(connectionString);

        var assembly = Assembly.GetExecutingAssembly();
        var resourceNames = assembly.GetManifestResourceNames();
        var sqlScripts = new Dictionary<string, string>();

        string namespacePrefix = $"{assembly.GetName().Name}.Scripts.";

        foreach (var resourceName in resourceNames)
        {
            if (resourceName.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
            {
                using (var stream = assembly.GetManifestResourceStream(resourceName))
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    var sqlScript = reader.ReadToEnd();
                    sqlScripts.Add(resourceName.Replace(namespacePrefix, ""), sqlScript);
                }
            }
        }

        return new Repository<Command>(logger, sqlScripts, connectionFactory);
    })
    .AddSingleton<IDeleteService<Vehicle>, NoSqlService<Vehicle>>()
    .AddSingleton<ICreateService<Command>, SqlService<Command>>()
    .AddSingleton<ICreateService<Vehicle>, NoSqlService<Vehicle>>()
    .AddSingleton<IService<Command>, SqlService<Command>>()
    .AddTransient<IPeriodicTimer>(service => {

        var periodicTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(500));

        return new Rent.Vehicles.Consumers.Utils.PeriodicTimer(periodicTimer);
    })
    .AddSingleton<IPublisher, Publisher>()
    .AddSingleton<ISerializer, MessagePackSerializer>()
    .AddHostedService<CreateVehiclesCommandBackgroundService>()
    .AddHostedService<DeleteVehiclesCommandBackgroundService>()
    .AddHostedService<VehiclesYearEventBackgroundService>()
    .AddHostedService<CreateVehiclesEventBackgroundService>()
    .AddHostedService<DeleteVehiclesEventBackgroundService>()
    .AddHostedService<CreateVehiclesYearEventBackgroundService>();

var host = builder.Build();
host.Run();

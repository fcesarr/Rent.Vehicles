using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Lib.Serializers;
using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services;
using Rent.Vehicles.Consumers.RabbitMQ.BackgroundServices;
using RabbitMQ.Client;
using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Repositories.Interfaces;
using Rent.Vehicles.Services.Repositories;
using Npgsql;
using System.Reflection;
using System.Text;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IModel>(service => {
    var factory = new ConnectionFactory { HostName = "localhost", Port = 5672, UserName = "admin", Password = "nimda" };
    var connection = factory.CreateConnection();
    return connection.CreateModel();
});

builder.Services.AddSingleton<IRepository<Command>, Repository<Command>>(service =>
{
    var logger = service.GetRequiredService<ILogger<Repository<Command>>>();

    var configuration = service.GetRequiredService<IConfiguration>();

    var connectionString = configuration.GetConnectionString("Database");

    var connectionFactory = new ConnectionFactory<NpgsqlConnection>(connectionString);

    var assembly = Assembly.GetExecutingAssembly();
    var resourceNames = assembly.GetManifestResourceNames();
    var sqlScripts = new Dictionary<string, string>();

    foreach (var resourceName in resourceNames)
    {
        if (resourceName.EndsWith(".sql", StringComparison.OrdinalIgnoreCase))
        {
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                var sqlScript = reader.ReadToEnd();
                sqlScripts.Add(resourceName, sqlScript);
            }
        }
    }

    return new Repository<Command>(logger, sqlScripts, connectionFactory);
});

builder.Services.AddSingleton<ICreateService<Command>, Service<Command>>();

builder.Services.AddHostedService<CreateBackgroundService<CreateVehiclesCommand, Command>>();

builder.Services.AddSingleton<IPeriodicTimer>(service => {

    var periodicTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(500));

    return new Rent.Vehicles.Consumers.Utils.PeriodicTimer(periodicTimer);
});

builder.Services.AddSingleton<ISerializer, MessagePackSerializer>();

var host = builder.Build();
host.Run();

using System.Reflection;
using System.Text;

using Microsoft.AspNetCore.Mvc;

using MongoDB.Driver;

using Npgsql;

using RabbitMQ.Client;

using Rent.Vehicles.Entities;
using Rent.Vehicles.Lib.Serializers;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Producers.RabbitMQ;
using Rent.Vehicles.Services;
using Rent.Vehicles.Services.Factories;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Services.Repositories;
using Rent.Vehicles.Services.Repositories.Interfaces;
using Rent.Vehicles.Services.Validators;
using Rent.Vehicles.Services.Validators.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IPublisher, Publisher>()
    .AddSingleton<ISerializer, MessagePackSerializer>()
    .AddSingleton<IModel>(service => {
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
    .AddSingleton<IValidator<Event>,  EventValidator>()
    .AddSingleton<IRepository<Event>, MongoRepository<Event>>()
    .AddSingleton<IFindService<Event>, NoSqlService<Event>>()
    .AddSingleton<IValidator<Vehicle>, VehicleValidator>()
    .AddSingleton<IRepository<Vehicle>, MongoRepository<Vehicle>>()
    .AddSingleton<IGetService<Vehicle>, NoSqlService<Vehicle>>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapPost("/Vehicles", async ([FromBody]CreateVehiclesCommand command,
    IPublisher publisher,
    CancellationToken cancellationToken = default) =>
{
    command.Id = Guid.NewGuid();
    command.SagaId = Guid.NewGuid();

    await publisher.PublishCommandAsync(command, cancellationToken);

    string locationUri = $"/vehicles/status/{command.SagaId}";

    return Results.Accepted(locationUri, new { Id = command.Id });
})
.WithName("VehiclesPost")
.WithOpenApi();

app.MapPut("/Vehicles", async ([FromBody]UpdateVehiclesCommand command,
    IPublisher publisher,
    CancellationToken cancellationToken = default) =>
{
    command.SagaId = Guid.NewGuid();

    await publisher.PublishCommandAsync(command, cancellationToken);

    string locationUri = $"/vehicles/status/{command.SagaId}";

    return Results.Accepted(locationUri);
})
.WithName("VehiclesPut")
.WithOpenApi();

app.MapDelete("/Vehicles", async ([FromBody]DeleteVehiclesCommand command,
    IPublisher publisher,
    CancellationToken cancellationToken = default) =>
{
    command.SagaId = Guid.NewGuid();

    await publisher.PublishCommandAsync(command!, cancellationToken);

    string locationUri = $"/vehicles/status/{command.SagaId}";

    return Results.Accepted(locationUri);
})
.WithName("VehiclesDelete")
.WithOpenApi();

app.MapGet("/Vehicles/{Id}", async ([FromQuery]Guid id,
    IGetService<Vehicle> getService,
    CancellationToken cancellationToken = default) =>
{
    var entity = await getService.GetAsync(x => x.Id == id, cancellationToken);

    if(entity == null)
        return Results.NotFound();

    return Results.Ok(entity);
})
.WithName("VehiclesGet")
.WithOpenApi();

app.MapGet("/Vehicles/Status/{SagaId}", async ([FromQuery]Guid sagaId,
    IFindService<Event> findService,
    CancellationToken cancellationToken = default) =>
{
    var entities = await findService.FindAsync(x => x.SagaId == sagaId, cancellationToken);

    if(!entities.Any())
        return Results.NoContent();

    return Results.Ok(entities);
})
.WithName("VehiclesStatus")
.WithOpenApi();


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

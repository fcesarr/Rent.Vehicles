using Microsoft.AspNetCore.Mvc;

using RabbitMQ.Client;

using Rent.Vehicles.Lib.Serializers;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Producers.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IPublisher, Publisher>()
    .AddSingleton<ISerializer, MessagePackSerializer>()
    .AddSingleton<IModel>(service => {
        var factory = new ConnectionFactory { HostName = "localhost", Port = 5672, UserName = "admin", Password = "nimda" };
        var connection = factory.CreateConnection();
        return connection.CreateModel();
    });

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

app.MapGet("/Vehicles/{Id}", ([FromQuery]Guid id,
    CancellationToken cancellationToken = default) =>
{
    return Results.Ok(new { id, status = "Processing" });
})
.WithName("VehiclesGet")
.WithOpenApi();

app.MapGet("/Vehicles/Status/{SagaId}", ([FromQuery]Guid sagaId,
    CancellationToken cancellationToken = default) =>
{
    return Results.Ok(new { sagaId, status = "Processing" });
})
.WithName("VehiclesStatus")
.WithOpenApi();


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

using Rent.Vehicles.Entities;
using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;
using Rent.Vehicles.Messages.Events;

namespace Rent.Vehicles.Consumers.RabbitMQ.Events.BackgroundServices;

public class CreateVehiclesSuccessEventNoSqlBackgroundService : HandlerEntityBackgroundService<
    CreateVehiclesSuccessEvent,
    Vehicle,
    IVehiclesService>
{
    public CreateVehiclesSuccessEventNoSqlBackgroundService(ILogger<CreateVehiclesSuccessEventNoSqlBackgroundService> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IVehiclesService service) : base(logger, channel, periodicTimer, serializer, service)
    {
        QueueName = $"{typeof(CreateVehiclesSuccessEvent).Name}.One";
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        var result = base.StartAsync(cancellationToken);

        _channel.QueueBind(queue: QueueName,
            exchange: typeof(CreateVehiclesSuccessEvent).Name, routingKey:"");
        
        return result;
    }

    protected override async Task HandlerAsync(CreateVehiclesSuccessEvent @event, CancellationToken cancellationToken = default)
    {
        await _service.CreateAsync(new Vehicle
        {
            Id = @event.Id,
            Year = @event.Year,
            Model = @event.Model,
            LicensePlate = @event.LicensePlate,
            Type = @event.Type
        }, cancellationToken);
    }
}

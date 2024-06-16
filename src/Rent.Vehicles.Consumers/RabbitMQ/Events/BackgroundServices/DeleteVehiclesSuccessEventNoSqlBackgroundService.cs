using Rent.Vehicles.Entities;
using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;
using Rent.Vehicles.Messages.Events;

namespace Rent.Vehicles.Consumers.RabbitMQ.Events.BackgroundServices;

public class DeleteVehiclesSuccessEventNoSqlBackgroundService : HandlerEntityNoSqlBackgroundService<
    DeleteVehiclesSuccessEvent,
    Vehicle>
{
    public DeleteVehiclesSuccessEventNoSqlBackgroundService(ILogger<DeleteVehiclesSuccessEventNoSqlBackgroundService> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        INoSqlService<Vehicle> service) : base(logger, channel, periodicTimer, serializer, service)
    {
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        var result = base.StartAsync(cancellationToken);

        _channel.QueueBind(queue: QueueName,
            exchange: typeof(DeleteVehiclesSuccessEvent).Name, routingKey:"");
        
        return result;
    }

    protected override async Task HandlerAsync(DeleteVehiclesSuccessEvent @event, CancellationToken cancellationToken = default)
    {
        await _service.DeleteAsync(@event.Id, cancellationToken);
    }
}

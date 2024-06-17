using Rent.Vehicles.Entities;
using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Producers.Interfaces;

namespace Rent.Vehicles.Consumers.RabbitMQ.Events.BackgroundServices;

public class DeleteVehiclesEventSqlBackgroundService : HandlerMessageAndActionAndPublisherBackgroundService<
    DeleteVehiclesEvent,
    DeleteVehiclesSuccessEvent,
    Vehicle,
    IVehiclesService>
{
    public DeleteVehiclesEventSqlBackgroundService(ILogger<DeleteVehiclesEventSqlBackgroundService> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IVehiclesService service,
        bool singleEvent = false) : base(logger, channel, periodicTimer, serializer, publisher, service, singleEvent)
    {
    }

    protected override DeleteVehiclesSuccessEvent CommandToEvent(DeleteVehiclesEvent @event)
    {
        return new DeleteVehiclesSuccessEvent
        {
            Id = @event.Id,
            SagaId = @event.SagaId
        };
    }

    protected override async Task HandlerMessageAsync(DeleteVehiclesEvent @event, CancellationToken cancellationToken = default)
    {
        await _service.DeleteAsync(@event.Id, cancellationToken);
    }
}
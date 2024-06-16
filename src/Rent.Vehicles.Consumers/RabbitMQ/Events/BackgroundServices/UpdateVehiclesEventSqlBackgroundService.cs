using Rent.Vehicles.Entities;
using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Services;
using Rent.Vehicles.Producers.Interfaces;

namespace Rent.Vehicles.Consumers.RabbitMQ.Events.BackgroundServices;

public class UpdateVehiclesEventSqlBackgroundService : HandlerEntitySqlPublisherBackgroundService<
    UpdateVehiclesEvent,
    UpdateVehiclesSuccessEvent,
    Vehicle>
{
    public UpdateVehiclesEventSqlBackgroundService(ILogger<UpdateVehiclesEventSqlBackgroundService> logger, IModel channel, IPeriodicTimer periodicTimer, ISerializer serializer, IPublisher publisher, ISqlService<Vehicle> service, bool singleEvent = false) : base(logger, channel, periodicTimer, serializer, publisher, service, singleEvent)
    {
    }

    protected override UpdateVehiclesSuccessEvent CommandToEvent(UpdateVehiclesEvent @event)
    {
        return new UpdateVehiclesSuccessEvent
        {
            Id = @event.Id,
            LicensePlate = @event.LicensePlate,
            SagaId = @event.SagaId
        };
    }

    protected override async Task HandlerMessageAsync(UpdateVehiclesEvent @event, CancellationToken cancellationToken = default)
    {
        var entity = await _service.GetAsync(x => x.Id == @event.Id, cancellationToken);

        if(entity == null)
            return;
        
        entity.LicensePlate = @event.LicensePlate;

        await _service.UpdateAsync(entity, cancellationToken);
    }
}


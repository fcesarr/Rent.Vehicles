using Rent.Vehicles.Entities;
using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Producers.Interfaces;

namespace Rent.Vehicles.Consumers.RabbitMQ.Events.BackgroundServices;

public class CreateVehiclesEventSqlBackgroundService : HandlerMessageAndActionAndPublisherBackgroundService<
    CreateVehiclesEvent,
    CreateVehiclesSuccessEvent,
    Vehicle,
    IVehiclesService>
{
    public CreateVehiclesEventSqlBackgroundService(ILogger<CreateVehiclesEventSqlBackgroundService> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IVehiclesService service) : base(logger, channel, periodicTimer, serializer, publisher, service)
    {
    }

    protected override CreateVehiclesSuccessEvent CommandToEvent(CreateVehiclesEvent @event)
    {
        return new CreateVehiclesSuccessEvent
        {
            Id = @event.Id,
            Year = @event.Year,
            Model = @event.Model,
            LicensePlate = @event.LicensePlate,
            Type = @event.Type,
            SagaId = @event.SagaId
        };
    }

    protected override async Task HandlerMessageAsync(CreateVehiclesEvent @event, CancellationToken cancellationToken = default)
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

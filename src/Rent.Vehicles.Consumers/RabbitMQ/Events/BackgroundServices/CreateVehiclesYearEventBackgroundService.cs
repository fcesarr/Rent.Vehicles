using Rent.Vehicles.Entities;
using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Producers.Interfaces;

namespace Rent.Vehicles.Consumers.RabbitMQ.Events.BackgroundServices;

public class CreateVehiclesYearEventBackgroundService : HandlerConsumerMessageBackgroundService<CreateVehiclesEvent>
{
    private readonly IPublisher _publisher;

    public CreateVehiclesYearEventBackgroundService(ILogger<CreateVehiclesYearEventBackgroundService> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher) : base(logger, channel, periodicTimer, serializer, "CreateVehiclesYearEvent")
    {
        _publisher = publisher;
    }

    protected override async Task HandlerAsync(CreateVehiclesEvent @event, CancellationToken cancellationToken = default)
    {
        if(@event.Year != 2024)
            return;

        await _publisher.PublishSingleEventAsync(new VehiclesYearEvent
            {
                Id = @event.Id, 
                Year = @event.Year,
                Model = @event.Model,
                LicensePlate = @event.LicensePlate,
                Type = @event.Type,
                SagaId = @event.SagaId
            }, cancellationToken);
    }
}
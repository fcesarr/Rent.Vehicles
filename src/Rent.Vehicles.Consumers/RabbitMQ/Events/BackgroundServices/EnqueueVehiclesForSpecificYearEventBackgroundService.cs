using Rent.Vehicles.Entities;
using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Producers.Interfaces;

namespace Rent.Vehicles.Consumers.RabbitMQ.Events.BackgroundServices;

public class EnqueueVehiclesForSpecificYearEventBackgroundService : HandlerConsumerMessageBackgroundService<CreateVehiclesEvent>
{
    private readonly IPublisher _publisher;

    public EnqueueVehiclesForSpecificYearEventBackgroundService(ILogger<EnqueueVehiclesForSpecificYearEventBackgroundService> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher) : base(logger, channel, periodicTimer, serializer, "EnqueueVehiclesYearEvent")
    {
        _publisher = publisher;
    }

    protected override async Task HandlerAsync(CreateVehiclesEvent @event, CancellationToken cancellationToken = default)
    {
        if(@event.Year != 2024)
            return;

        await _publisher.PublishSingleEventAsync(new CreateVehiclesForSpecificYearEvent
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
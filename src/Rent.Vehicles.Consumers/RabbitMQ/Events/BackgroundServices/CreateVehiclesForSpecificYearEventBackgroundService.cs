using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Producers.Interfaces;

namespace Rent.Vehicles.Consumers.RabbitMQ.Events.BackgroundServices;

public class CreateVehiclesForSpecificYearEventBackgroundService : HandlerConsumerMessageBackgroundService<CreateVehiclesForSpecificYearEvent>
{
    private readonly IPublisher _publisher;

    public CreateVehiclesForSpecificYearEventBackgroundService(ILogger<CreateVehiclesForSpecificYearEventBackgroundService> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher) : base(logger, channel, periodicTimer, serializer, "CreateVehiclesForSpecificYearEvent")
    {
        _publisher = publisher;
    }

    protected override async Task HandlerAsync(CreateVehiclesForSpecificYearEvent @event, CancellationToken cancellationToken = default)
    {
        await Task.Run(() => {
            _logger.LogInformation("CreateVehiclesForSpecificYearEvent {id}", @event.Id);
        }, cancellationToken);
    }
}
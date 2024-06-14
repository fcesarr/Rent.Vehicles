using Rent.Vehicles.Entities;
using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Producers.Interfaces;

namespace Rent.Vehicles.Consumers.RabbitMQ.Events.BackgroundServices;

public class CreateVehiclesEventBackgroundService : HandlerConsumerEventBackgroundService<CreateVehiclesEvent>
{
    protected readonly ICreateService<Vehicle> _createService;

    private readonly IPublisher _publisher;

    public CreateVehiclesEventBackgroundService(ILogger<CreateVehiclesEventBackgroundService> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        ICreateService<Vehicle> createService,
        ICreateService<Event> createEventService,
        IPublisher publisher) : base(logger, channel, periodicTimer, serializer, createEventService)
    {
        _createService = createService;
        _publisher = publisher;
    }

    protected override async Task HandlerEventAsync(CreateVehiclesEvent @event, CancellationToken cancellationToken = default)
    {
        await _createService.CreateAsync(new Vehicle
        {
            Id = @event.Id,
            Year = @event.Year,
            Model = @event.Model,
            LicensePlate = @event.LicensePlate,
            Type = @event.Type
        }, cancellationToken);

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

public class CreateVehiclesEventProjection
{
    
}
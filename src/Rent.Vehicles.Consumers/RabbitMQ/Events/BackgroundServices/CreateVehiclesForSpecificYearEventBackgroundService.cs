using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Interfaces;

namespace Rent.Vehicles.Consumers.RabbitMQ.Events.BackgroundServices;

public class CreateVehiclesForSpecificYearEventBackgroundService : HandlerConsumerEventBackgroundService<CreateVehiclesForSpecificYearEvent>
{
    protected readonly ICreateService<VehiclesForSpecificYear> _createService;

    public CreateVehiclesForSpecificYearEventBackgroundService(ILogger<CreateVehiclesForSpecificYearEventBackgroundService> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        ICreateService<VehiclesForSpecificYear> createService,
        ICreateService<Event> createEventService) : base(logger, channel, periodicTimer, serializer, createEventService)
    {
        _createService = createService;
    }

    protected override async Task HandlerEventAsync(CreateVehiclesForSpecificYearEvent @event, CancellationToken cancellationToken = default)
    {
        await _createService.CreateAsync(new VehiclesForSpecificYear
        {
            Id = @event.Id,
            Year = @event.Year,
            Model = @event.Model,
            LicensePlate = @event.LicensePlate,
            Type = @event.Type
        }, cancellationToken);
    }
}
using Rent.Vehicles.Entities;
using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Services;

namespace Rent.Vehicles.Consumers.RabbitMQ.Events.BackgroundServices;

public class UpdateVehiclesEventBackgroundService : HandlerConsumerEventBackgroundService<UpdateVehiclesEvent>
{
    protected readonly IVehicleService _updateService;

    public UpdateVehiclesEventBackgroundService(ILogger<UpdateVehiclesEventBackgroundService> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IVehicleService updateService,
        ICreateService<Event> createEventService) : base(logger, channel, periodicTimer, serializer, createEventService)
    {
        _updateService = updateService;
    }

    protected override async Task HandlerEventAsync(UpdateVehiclesEvent @event, CancellationToken cancellationToken = default)
    {
        await _updateService.UpdateAsync(@event.Id,
            @event.LicensePlate,
            cancellationToken);
    }
}


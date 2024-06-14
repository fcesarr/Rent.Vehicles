using Rent.Vehicles.Entities;
using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;
using Rent.Vehicles.Messages.Events;

namespace Rent.Vehicles.Consumers.RabbitMQ.Events.BackgroundServices;

public class UpdateVehiclesEventBackgroundService : HandlerConsumerEventToEntityBackgroundService<UpdateVehiclesEvent, Vehicle>
{
    protected readonly IUpdateService<Vehicle> _updateService;

    public UpdateVehiclesEventBackgroundService(ILogger<UpdateVehiclesEventBackgroundService> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IUpdateService<Vehicle> updateService,
        IBothServices<Event> createEventService) : base(logger, channel, periodicTimer, serializer, "UpdateVehiclesEvent", createEventService)
    {
        _updateService = updateService;
    }

    protected override async Task<Vehicle> EventToEntityAsync(UpdateVehiclesEvent message, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() => new Vehicle
        {
            Id = message.Id,
            LicensePlate = message.LicensePlate,
        }, cancellationToken);
    }

    protected override async Task HandlerAsync(Vehicle entity, CancellationToken cancellationToken = default)
    {
        await _updateService.UpdateAsync(entity, cancellationToken);
    }
}
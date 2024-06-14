using Rent.Vehicles.Entities;
using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;
using Rent.Vehicles.Messages.Events;

namespace Rent.Vehicles.Consumers.RabbitMQ.Events.BackgroundServices;

public class DeleteVehiclesEventBackgroundService : HandlerConsumerEventToEntityBackgroundService<DeleteVehiclesEvent, Vehicle>
{
    protected readonly IDeleteService<Vehicle> _deleteService;

    public DeleteVehiclesEventBackgroundService(ILogger<DeleteVehiclesEventBackgroundService> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IDeleteService<Vehicle> deleteService,
        ICreateService<Event> createEventService) : base(logger, channel, periodicTimer, serializer, "DeleteVehiclesEvent", createEventService)
    {
        _deleteService = deleteService;
    }

    protected override async Task<Vehicle> EventToEntityAsync(DeleteVehiclesEvent message, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() => new Vehicle
        {
            Id = message.Id
        }, cancellationToken);
    }

    protected override async Task HandlerAsync(Vehicle entity, CancellationToken cancellationToken = default)
    {
        await _deleteService.DeleteAsync(entity, cancellationToken);
    }
}
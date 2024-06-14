using Rent.Vehicles.Entities;
using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;
using Rent.Vehicles.Messages.Events;

namespace Rent.Vehicles.Consumers.RabbitMQ.Events.BackgroundServices;

public class DeleteVehiclesEventBackgroundService : HandlerConsumerEventBackgroundService<DeleteVehiclesEvent>
{
    protected readonly IDeleteService<Vehicle> _deleteService;

    public DeleteVehiclesEventBackgroundService(ILogger<DeleteVehiclesEventBackgroundService> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IDeleteService<Vehicle> deleteService,
        ICreateService<Event> createEventService) : base(logger, channel, periodicTimer, serializer, createEventService)
    {
        _deleteService = deleteService;
    }

    protected override async Task HandlerEventAsync(DeleteVehiclesEvent @event, CancellationToken cancellationToken = default)
    {
        await _deleteService.DeleteAsync(@event.Id, cancellationToken);
    }
}
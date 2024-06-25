using Rent.Vehicles.Entities;
using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Consumers.Handlers.BackgroundServices;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Entities.Projections;
using Rent.Vehicles.Services.DataServices.Interfaces;
using Rent.Vehicles.Consumers.Interfaces;
using Rent.Vehicles.Services;

namespace Rent.Vehicles.Consumers.Events.BackgroundServices;

public class DeleteVehiclesProjectionEventBackgroundService : HandlerEventServicePublishBackgroundService<
    DeleteVehiclesProjectionEvent,
    IVehicleProjectionDataService>
{
    public DeleteVehiclesProjectionEventBackgroundService(ILogger<DeleteVehiclesProjectionEventBackgroundService> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IVehicleProjectionDataService service) : base(logger, channel, periodicTimer, serializer, publisher, service)
    {
    }

    protected override async Task<Result<Task>> HandlerMessageAsync(DeleteVehiclesProjectionEvent @event, CancellationToken cancellationToken = default)
    {
        var entity =  await _service.DeleteAsync(@event.Id, cancellationToken);

        if(!entity.IsSuccess)
            return entity.Exception!;

        return Task.CompletedTask;
    }
}

using Rent.Vehicles.Entities;
using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Producers.Interfaces;

namespace Rent.Vehicles.Consumers.RabbitMQ.Events.BackgroundServices;

public class DeleteVehiclesSuccessEventNoSqlBackgroundService : HandlerEventServicePublishBackgroundService<
    DeleteVehiclesSuccessEvent,
    Vehicle,
    IVehiclesService>
{
    public DeleteVehiclesSuccessEventNoSqlBackgroundService(ILogger<DeleteVehiclesSuccessEventNoSqlBackgroundService> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IVehiclesService service) : base(logger, channel, periodicTimer, serializer, publisher, service)
    {
    }

    protected override async Task HandlerMessageAsync(DeleteVehiclesSuccessEvent @event, CancellationToken cancellationToken = default)
    {
        await _service.DeleteAsync(@event.Id, cancellationToken);
    }
}

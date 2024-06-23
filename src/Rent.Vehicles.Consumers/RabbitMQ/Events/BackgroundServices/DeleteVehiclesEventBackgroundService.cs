using Rent.Vehicles.Entities;
using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Producers.Interfaces;
using LanguageExt.Common;

namespace Rent.Vehicles.Consumers.RabbitMQ.Events.BackgroundServices;

public class DeleteVehiclesEventBackgroundService : HandlerEventServicePublishEventBackgroundService<
    DeleteVehiclesEvent,
    IVehicleDataService>
{
    public DeleteVehiclesEventBackgroundService(ILogger<DeleteVehiclesEventBackgroundService> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IVehicleDataService service) : base(logger, channel, periodicTimer, serializer, publisher, service)
    {
    }

    protected override IEnumerable<Messages.Event> CreateEventToPublish(DeleteVehiclesEvent @event)
    {
        return [
            new DeleteVehiclesProjectionEvent
            {
                Id = @event.Id,
                SagaId = @event.SagaId
            }
        ];
    }

    protected override async Task<Result<Task>> HandlerMessageAsync(DeleteVehiclesEvent @event, CancellationToken cancellationToken = default)
    {
        var entity = await _service.DeleteAsync(@event.Id, cancellationToken);

        return entity.Match(entity => Task.CompletedTask, exception => new Result<Task>(exception));
    }
}
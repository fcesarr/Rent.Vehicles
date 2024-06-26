using Rent.Vehicles.Entities;
using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Consumers.Handlers.BackgroundServices;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Consumers.Interfaces;
using Rent.Vehicles.Services;

namespace Rent.Vehicles.Consumers.Events.BackgroundServices;

public class DeleteVehiclesEventBackgroundService : HandlerEventServicePublishEventBackgroundService<
    DeleteVehiclesEvent>
{
    public DeleteVehiclesEventBackgroundService(ILogger<DeleteVehiclesEventBackgroundService> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IServiceScopeFactory serviceScopeFactory) : base(logger, channel, periodicTimer, serializer, publisher, serviceScopeFactory)
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
        var _service = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IVehicleDataService>();

        var entity = await _service.DeleteAsync(@event.Id, cancellationToken);

        if(!entity.IsSuccess)
            return entity.Exception!;

        return Task.CompletedTask;
    }
}
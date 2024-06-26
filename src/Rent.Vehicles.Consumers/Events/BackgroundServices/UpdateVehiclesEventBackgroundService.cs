using Rent.Vehicles.Entities;
using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Consumers.Handlers.BackgroundServices;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Services;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Consumers.Interfaces;

namespace Rent.Vehicles.Consumers.Events.BackgroundServices;

public class UpdateVehiclesEventBackgroundService : HandlerEventServicePublishEventBackgroundService<
    UpdateVehiclesEvent>
{
    public UpdateVehiclesEventBackgroundService(ILogger<UpdateVehiclesEventBackgroundService> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IServiceScopeFactory serviceScopeFactory) : base(logger, channel, periodicTimer, serializer, publisher, serviceScopeFactory)
    {
    }

    protected override IEnumerable<Messages.Event> CreateEventToPublish(UpdateVehiclesEvent @event)
    {
        return [
            new UpdateVehiclesProjectionEvent
            {
                Id = @event.Id,
                LicensePlate = @event.LicensePlate,
                SagaId = @event.SagaId
            }
        ];
    }

    protected override async Task<Result<Task>> HandlerMessageAsync(UpdateVehiclesEvent @event, CancellationToken cancellationToken = default)
    {
        var _service = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IVehicleDataService>();

        var entity = await _service.UpdateAsync(@event.Id, @event.LicensePlate, cancellationToken);

        if(!entity.IsSuccess)
            return entity.Exception!;

        return Task.CompletedTask;
    }
}


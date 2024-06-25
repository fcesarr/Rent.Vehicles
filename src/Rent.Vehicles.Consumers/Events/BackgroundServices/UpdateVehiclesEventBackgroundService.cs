using Rent.Vehicles.Entities;
using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Consumers.Handlers.BackgroundServices;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Services;
using Rent.Vehicles.Producers.Interfaces;
using LanguageExt.Common;
using Rent.Vehicles.Consumers.Interfaces;

namespace Rent.Vehicles.Consumers.Events.BackgroundServices;

public class UpdateVehiclesEventBackgroundService : HandlerEventServicePublishEventBackgroundService<
    UpdateVehiclesEvent,
    IVehicleDataService>
{
    public UpdateVehiclesEventBackgroundService(ILogger<UpdateVehiclesEventBackgroundService> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IVehicleDataService service) : base(logger, channel, periodicTimer, serializer, publisher, service)
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

    protected override async Task<LanguageExt.Common.Result<Task>> HandlerMessageAsync(UpdateVehiclesEvent @event, CancellationToken cancellationToken = default)
    {
        var entity = await _service.UpdateAsync(@event.Id, @event.LicensePlate, cancellationToken);

        if(!entity.IsSuccess)
            return new LanguageExt.Common.Result<Task>(entity.Exception);

        return Task.CompletedTask;
    }
}


using Rent.Vehicles.Entities;
using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Consumers.Handlers.BackgroundServices;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Producers.Interfaces;
using LanguageExt.Common;
using Rent.Vehicles.Services.DataServices.Interfaces;
using Rent.Vehicles.Consumers.Interfaces;

namespace Rent.Vehicles.Consumers.Events.BackgroundServices;

public class UpdateVehiclesProjectionEventBackgroundService : HandlerEventServicePublishBackgroundService<
    UpdateVehiclesProjectionEvent,
    IVehicleProjectionDataService>
{
    public UpdateVehiclesProjectionEventBackgroundService(ILogger<UpdateVehiclesProjectionEventBackgroundService> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IVehicleProjectionDataService service) : base(logger, channel, periodicTimer, serializer, publisher, service)
    {
    }

    protected override async Task<Result<Task>> HandlerMessageAsync(UpdateVehiclesProjectionEvent @event, CancellationToken cancellationToken = default)
    {
        var entity = await _service.UpdateAsync(@event.Id, @event.LicensePlate, cancellationToken);

        return entity.Match(entity => Task.CompletedTask, exception => new Result<Task>(exception));
    }
}

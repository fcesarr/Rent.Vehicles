using Rent.Vehicles.Consumers.Handlers.BackgroundServices;
using Rent.Vehicles.Consumers.Interfaces;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Entities.Projections;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Services;
using Rent.Vehicles.Services.DataServices.Interfaces;

namespace Rent.Vehicles.Consumers.Events.BackgroundServices;

public class UpdateVehiclesProjectionEventBackgroundService : HandlerEventServicePublishBackgroundService<
    UpdateVehiclesProjectionEvent>
{
    public UpdateVehiclesProjectionEventBackgroundService(
        ILogger<UpdateVehiclesProjectionEventBackgroundService> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IServiceScopeFactory serviceScopeFactory) : base(logger, channel, periodicTimer, serializer, publisher,
        serviceScopeFactory)
    {
    }

    protected override async Task<Result<Task>> HandlerMessageAsync(UpdateVehiclesProjectionEvent @event,
        CancellationToken cancellationToken = default)
    {
        IVehicleProjectionDataService _service = _serviceScopeFactory.CreateScope().ServiceProvider
            .GetRequiredService<IVehicleProjectionDataService>();

        Result<VehicleProjection>
            entity = await _service.UpdateAsync(@event.Id, @event.LicensePlate, cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!;
        }

        return Task.CompletedTask;
    }
}
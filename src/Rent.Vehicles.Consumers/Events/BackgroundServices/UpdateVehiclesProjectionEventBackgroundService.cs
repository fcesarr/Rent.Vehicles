using Rent.Vehicles.Consumers.Handlers.BackgroundServices;
using Rent.Vehicles.Consumers.Interfaces;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Messages.Projections.Events;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Services;
using Rent.Vehicles.Services.Facades.Interfaces;

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
        var _service = _serviceScopeFactory.CreateScope().ServiceProvider
            .GetRequiredService<IVehicleProjectionFacade>();

        var entity = await _service.UpdateAsync(@event, cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!;
        }

        return Task.CompletedTask;
    }
}

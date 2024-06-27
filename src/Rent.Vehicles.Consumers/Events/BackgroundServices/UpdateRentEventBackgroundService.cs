using Rent.Vehicles.Consumers.Handlers.BackgroundServices;
using Rent.Vehicles.Consumers.Interfaces;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Messages.Projections.Events;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Services;
using Rent.Vehicles.Services.Facades.Interfaces;

using Event = Rent.Vehicles.Messages.Event;

namespace Rent.Vehicles.Consumers.Events.BackgroundServices;

public class UpdateRentEventBackgroundService : HandlerEventServicePublishEventBackgroundService<
    UpdateRentEvent>
{
    public UpdateRentEventBackgroundService(ILogger<UpdateRentEventBackgroundService> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IServiceScopeFactory serviceScopeFactory) : base(logger, channel, periodicTimer, serializer, publisher,
        serviceScopeFactory)
    {
    }

    protected override IEnumerable<Event> CreateEventToPublish(UpdateRentEvent @event)
    {
        return
        [
            new UpdateRentProjectionEvent { Id = @event.Id, SagaId = @event.SagaId }
        ];
    }

    protected override async Task<Result<Task>> HandlerMessageAsync(UpdateRentEvent @event,
        CancellationToken cancellationToken = default)
    {
        var service = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IRentFacade>();

        var entity = await service.UpdateAsync(@event, cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!;
        }

        return Task.CompletedTask;
    }
}

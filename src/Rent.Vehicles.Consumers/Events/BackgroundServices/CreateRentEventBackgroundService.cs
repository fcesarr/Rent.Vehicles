using Rent.Vehicles.Consumers.Handlers.BackgroundServices;
using Rent.Vehicles.Consumers.Interfaces;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Services;
using Rent.Vehicles.Services.Facades.Interfaces;

using Event = Rent.Vehicles.Messages.Event;

namespace Rent.Vehicles.Consumers.Events.BackgroundServices;

public class CreateRentEventBackgroundService : HandlerEventServicePublishEventBackgroundService<
    CreateRentEvent>
{
    public CreateRentEventBackgroundService(ILogger<CreateRentEventBackgroundService> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IServiceScopeFactory serviceScopeFactory) : base(logger, channel, periodicTimer, serializer, publisher,
        serviceScopeFactory)
    {
    }

    protected override IEnumerable<Event> CreateEventToPublish(CreateRentEvent @event)
    {
        return
        [
        ];
    }

    protected override async Task<Result<Task>> HandlerMessageAsync(CreateRentEvent @event,
        CancellationToken cancellationToken = default)
    {
        var _service = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IRentFacade>();

        var user = await _service.CreateAsync(@event, cancellationToken);

        if (!user.IsSuccess)
        {
            return user.Exception!;
        }

        return Task.CompletedTask;
    }
}

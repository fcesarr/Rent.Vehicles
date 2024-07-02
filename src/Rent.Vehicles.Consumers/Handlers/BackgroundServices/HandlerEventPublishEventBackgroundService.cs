using Rent.Vehicles.Consumers.Interfaces;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Messages;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Services;

namespace Rent.Vehicles.Consumers.Handlers.BackgroundServices;

public abstract class
    HandlerEventPublishEventBackgroundService<TEventToConsume> : HandlerEventPublishBackgroundService<TEventToConsume>
    where TEventToConsume : Event
{
    protected HandlerEventPublishEventBackgroundService(
        ILogger<HandlerEventPublishEventBackgroundService<TEventToConsume>> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher) : base(logger, channel, periodicTimer, serializer, publisher)
    {
    }

    protected override async Task<Result<Task>> HandlerAsync(TEventToConsume @event,
        CancellationToken cancellationToken = default)
    {
        var result = await base.HandlerAsync(@event, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Exception!;
        }

        await result.Value!;

        var eventsToPublish = CreateEventToPublish(@event);

        return PublishAsync(eventsToPublish, cancellationToken);
    }

    protected abstract IEnumerable<Event> CreateEventToPublish(TEventToConsume @event);

    protected virtual async Task PublishAsync(IEnumerable<Event> events, CancellationToken cancellationToken = default)
    {
        List<Task> tasks = new();

        foreach (var @event in events)
        {
            tasks.Add(_publisher.PublishSingleEventAsync(@event, cancellationToken));
        }

        await Task.WhenAll(tasks);
    }
}

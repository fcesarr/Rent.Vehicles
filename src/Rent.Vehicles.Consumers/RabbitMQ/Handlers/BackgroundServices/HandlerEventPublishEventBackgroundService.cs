using RabbitMQ.Client;

using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Producers.Interfaces;

namespace Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;

public abstract class HandlerEventPublishEventBackgroundService<TEventToConsume, TEventToPublish> : HandlerEventPublishBackgroundService<TEventToConsume>
    where TEventToConsume : Messages.Event
    where TEventToPublish : Messages.Event
{
    protected HandlerEventPublishEventBackgroundService(ILogger<HandlerEventPublishEventBackgroundService<TEventToConsume, TEventToPublish>> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher) : base(logger, channel, periodicTimer, serializer, publisher)
    {
    }

    protected override async Task HandlerAsync(TEventToConsume @event, CancellationToken cancellationToken = default)
    {
        await base.HandlerAsync(@event, cancellationToken);

        var eventToPublish = CreateEventToPublish(@event);
        
        await PublishAsync(eventToPublish, cancellationToken);
    }

    protected abstract TEventToPublish CreateEventToPublish(TEventToConsume @event);

    protected virtual async Task PublishAsync(TEventToPublish @event, CancellationToken cancellationToken = default)
    {
        await _publisher.PublishSingleEventAsync(@event, cancellationToken);
    }
}
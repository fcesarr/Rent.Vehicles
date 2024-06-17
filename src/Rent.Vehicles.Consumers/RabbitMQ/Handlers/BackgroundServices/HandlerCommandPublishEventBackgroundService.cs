
using RabbitMQ.Client;

using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Producers.Interfaces;

namespace Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;

public abstract class HandlerCommandPublishEventBackgroundService<TCommandToConsume, TEventToPublish> : HandlerCommandPublishBackgroundService<TCommandToConsume>
    where TCommandToConsume : Messages.Command
    where TEventToPublish : Messages.Event
{
    protected HandlerCommandPublishEventBackgroundService(ILogger<HandlerCommandPublishEventBackgroundService<TCommandToConsume, TEventToPublish>> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher) : base(logger, channel, periodicTimer, serializer, publisher)
    {
    }

    protected override async Task HandlerAsync(TCommandToConsume @event, CancellationToken cancellationToken = default)
    {
        await base.HandlerAsync(@event, cancellationToken);

        var eventToPublish = CreateEventToPublish(@event);
        
        await PublishAsync(eventToPublish, cancellationToken);
    }

    protected abstract TEventToPublish CreateEventToPublish(TCommandToConsume @event);

    protected virtual async Task PublishAsync(TEventToPublish @event, CancellationToken cancellationToken = default)
    {
        await _publisher.PublishSingleEventAsync(@event, cancellationToken);
    }
}
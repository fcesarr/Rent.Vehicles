
using RabbitMQ.Client;

using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Entities;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Services.Interfaces;

namespace Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;

public abstract class HandlerMessageAndPublisherBackgroundService<TMessageToConsume, TEventToPublisher> : HandlerMessageBackgroundService<TMessageToConsume> 
    where TMessageToConsume : Messages.Message
    where TEventToPublisher : Messages.Event
{
    protected readonly IPublisher _publisher;

    private readonly bool _singleEvent;

    protected HandlerMessageAndPublisherBackgroundService(ILogger<HandlerMessageAndPublisherBackgroundService<TMessageToConsume, TEventToPublisher>> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        bool singleEvent = false) : base(logger, channel, periodicTimer, serializer)
    {
        _publisher = publisher;
        _singleEvent = singleEvent;
    }

    protected abstract TEventToPublisher CommandToEvent(TMessageToConsume command);

    protected override async Task HandlerAsync(TMessageToConsume command, CancellationToken cancellationToken = default)
    {
        await HandlerMessageAsync(command, cancellationToken);

        var @event = CommandToEvent(command);

        if(_singleEvent)
            await _publisher.PublishSingleEventAsync(@event, cancellationToken);
        else
            await _publisher.PublishEventAsync(@event, cancellationToken);
    }

    protected abstract Task HandlerMessageAsync(TMessageToConsume command, CancellationToken cancellationToken = default);
}
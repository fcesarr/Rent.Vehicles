using RabbitMQ.Client;

using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Messages;
using Rent.Vehicles.Producers.Interfaces;

using Event = Rent.Vehicles.Messages.Events.Event;

namespace Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;

public abstract class HandlerCommandPublishBackgroundService<TCommandToConsume> : HandlerMessageBackgroundService<TCommandToConsume>
    where TCommandToConsume : Messages.Command
{
    protected readonly IPublisher _publisher;

    protected HandlerCommandPublishBackgroundService(ILogger<HandlerCommandPublishBackgroundService<TCommandToConsume>> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher) : base(logger, channel, periodicTimer, serializer)
    {
        _publisher = publisher;
    }

    protected override async Task HandlerAsync(TCommandToConsume eventToPublish, CancellationToken cancellationToken = default)
    {
        await HandlerMessageAsync(eventToPublish, cancellationToken);
    }

    protected abstract Task HandlerMessageAsync(TCommandToConsume @event, CancellationToken cancellationToken = default);

    
}
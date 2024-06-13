
using RabbitMQ.Client;

using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Producers.Interfaces;

namespace Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;

public abstract class HandlerConsumerCommandToEntityPublisherBackgroundService<TCommand, TEvent, TEntity> : HandlerConsumerMessageBackgroundService<TCommand> 
    where TCommand : Messages.Command
    where TEvent : Messages.Event
    where TEntity : Entities.Command
{
    protected readonly IPublisher _publisher;

    protected HandlerConsumerCommandToEntityPublisherBackgroundService(ILogger<HandlerConsumerCommandToEntityPublisherBackgroundService<TCommand, TEvent, TEntity>> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        string queueName,
        IPublisher publisher) : base(logger, channel, periodicTimer, serializer, queueName)
    {
        _publisher = publisher;
    }

    protected abstract Task<TEntity> CommandToEntityAsync(TCommand message, ISerializer serializer, CancellationToken cancellationToken = default);

    protected abstract Task<TEvent> CommandToEventAsync(TCommand message, CancellationToken cancellationToken = default);

    protected override async Task HandlerAsync(TCommand command, CancellationToken cancellationToken = default)
    {
        var entity = await CommandToEntityAsync(command, _serializer, cancellationToken);

        await HandlerAsync(entity, cancellationToken);

        var @event = await CommandToEventAsync(command, cancellationToken);

        await _publisher.PublishEventAsync(@event, cancellationToken);
    }

    protected abstract Task HandlerAsync(TEntity entity, CancellationToken cancellationToken = default);
}
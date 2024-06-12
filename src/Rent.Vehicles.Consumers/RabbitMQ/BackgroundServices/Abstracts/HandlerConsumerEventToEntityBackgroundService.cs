
using RabbitMQ.Client;

using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;

namespace Rent.Vehicles.Consumers.RabbitMQ.BackgroundServices.Abstracts;

public abstract class HandlerConsumerEventToEntityBackgroundService<TEvent, TEntity> : HandlerConsumerMessageBackgroundService<TEvent> 
    where TEvent : Messages.Event
    where TEntity : Entities.Entity
{
    protected HandlerConsumerEventToEntityBackgroundService(ILogger<HandlerConsumerEventToEntityBackgroundService<TEvent, TEntity>> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        string queueName) : base(logger, channel, periodicTimer, serializer, queueName)
    {
    }

    protected abstract Task<TEntity> EventToEntityAsync(TEvent @event, CancellationToken cancellationToken = default);

    protected override async Task HandlerAsync(TEvent @event, CancellationToken cancellationToken = default)
    {
        var entity = await EventToEntityAsync(@event, cancellationToken);

        await HandlerAsync(entity, cancellationToken);
    }

    protected abstract Task HandlerAsync(TEntity entity, CancellationToken cancellationToken = default);
}
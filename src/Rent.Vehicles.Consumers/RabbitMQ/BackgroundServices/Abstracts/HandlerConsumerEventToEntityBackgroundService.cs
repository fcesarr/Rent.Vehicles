
using RabbitMQ.Client;

using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;

namespace Rent.Vehicles.Consumers.RabbitMQ.BackgroundServices.Abstracts;

public abstract class HandlerConsumerEventToEntityBackgroundService<TEvent, TEntity> : HandlerConsumerEventBackgroundService<TEvent, TEntity> 
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

    protected abstract Task<TEntity> EventToEntityAsync(TEvent message, CancellationToken cancellationToken = default);
}
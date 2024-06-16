

using RabbitMQ.Client;

using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Entities;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Services.Interfaces;

namespace Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;

public abstract class HandlerEntitySqlPublisherBackgroundService<TCommandToConsume, TEventToPublisher, TEntity> : HandlerMessageAndPublisherBackgroundService<TCommandToConsume, TEventToPublisher> 
    where TCommandToConsume : Messages.Message
    where TEventToPublisher : Messages.Event
    where TEntity : Entity
{
    protected readonly ISqlService<TEntity> _service;

    protected HandlerEntitySqlPublisherBackgroundService(ILogger<HandlerMessageAndPublisherBackgroundService<TCommandToConsume, TEventToPublisher>> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        ISqlService<TEntity> service,
        bool singleEvent = false) : base(logger, channel, periodicTimer, serializer, publisher, singleEvent)
    {
        _service = service;
    }
}
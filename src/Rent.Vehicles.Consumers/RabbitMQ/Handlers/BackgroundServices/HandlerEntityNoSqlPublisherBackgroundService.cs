
using RabbitMQ.Client;

using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Entities;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Services.Interfaces;

namespace Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;

public abstract class HandlerEntityNoSqlPublisherBackgroundService<TEventToConsume, TEventToPublisher, TEntity> : HandlerMessageAndPublisherBackgroundService<TEventToConsume, TEventToPublisher> 
    where TEventToConsume : Messages.Event
    where TEventToPublisher : Messages.Event
    where TEntity : Entity
{
    protected readonly INoSqlService<TEntity> _service;

    protected HandlerEntityNoSqlPublisherBackgroundService(ILogger<HandlerEntityNoSqlPublisherBackgroundService<TEventToConsume, TEventToPublisher, TEntity>> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        INoSqlService<TEntity> service,
        bool singleEvent = false) : base(logger, channel, periodicTimer, serializer, publisher, singleEvent)
    {
        _service = service;
    }
}
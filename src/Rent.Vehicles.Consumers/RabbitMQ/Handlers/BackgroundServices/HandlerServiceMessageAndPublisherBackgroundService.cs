

using RabbitMQ.Client;

using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Entities;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Services.Interfaces;

namespace Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;

public abstract class HandlerServiceMessageAndPublisherBackgroundService<TMessageToConsume, TEventToPublisher, TEntity, TService> : HandlerMessageAndPublisherBackgroundService<TMessageToConsume, TEventToPublisher> 
    where TMessageToConsume : Messages.Message
    where TEventToPublisher : Messages.Event
    where TEntity : Entity
    where TService : IService<TEntity>
{
    protected readonly TService _service;

    protected HandlerServiceMessageAndPublisherBackgroundService(ILogger<HandlerServiceMessageAndPublisherBackgroundService<TMessageToConsume, TEventToPublisher, TEntity, TService>> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        TService service,
        bool singleEvent = false) : base(logger, channel, periodicTimer, serializer, publisher, singleEvent)
    {
        _service = service;
    }
}
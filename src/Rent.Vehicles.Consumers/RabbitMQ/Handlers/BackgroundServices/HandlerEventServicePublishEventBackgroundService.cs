using RabbitMQ.Client;

using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Producers.Interfaces;

namespace Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;

public abstract class HandlerEventServicePublishEventBackgroundService<TEventToConsume, TEventToPublish, TService> : HandlerEventPublishEventBackgroundService<TEventToConsume, TEventToPublish>
    where TEventToConsume : Messages.Event
    where TEventToPublish : Messages.Event
    where TService : class
{
    protected readonly TService _service;

    protected HandlerEventServicePublishEventBackgroundService(ILogger<HandlerEventServicePublishEventBackgroundService<TEventToConsume, TEventToPublish, TService>> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        TService service) : base(logger, channel, periodicTimer, serializer, publisher)
    {
        _service = service;
    }
}
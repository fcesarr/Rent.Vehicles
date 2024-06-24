using RabbitMQ.Client;

using Rent.Vehicles.Consumers.Interfaces;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Producers.Interfaces;

namespace Rent.Vehicles.Consumers.Handlers.BackgroundServices;

public abstract class HandlerEventServicePublishEventBackgroundService<TEventToConsume, TService> : HandlerEventPublishEventBackgroundService<TEventToConsume>
    where TEventToConsume : Messages.Event
    where TService : class
{
    protected readonly TService _service;

    protected HandlerEventServicePublishEventBackgroundService(ILogger<HandlerEventServicePublishEventBackgroundService<TEventToConsume, TService>> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        TService service) : base(logger, channel, periodicTimer, serializer, publisher)
    {
        _service = service;
    }
}
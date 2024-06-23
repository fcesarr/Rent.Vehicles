using RabbitMQ.Client;

using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Producers.Interfaces;

namespace Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;

public abstract class HandlerEventServicePublishBackgroundService<TEventToConsume, TService> : HandlerEventPublishBackgroundService<TEventToConsume>
    where TEventToConsume : Messages.Event
    where TService : class
{
    protected readonly TService _service;

    protected HandlerEventServicePublishBackgroundService(ILogger<HandlerEventServicePublishBackgroundService<TEventToConsume, TService>> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        TService service) : base(logger, channel, periodicTimer, serializer, publisher)
    {
        _service = service;
    }


}
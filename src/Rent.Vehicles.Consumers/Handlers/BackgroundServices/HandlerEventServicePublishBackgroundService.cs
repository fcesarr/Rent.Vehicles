using RabbitMQ.Client;

using Rent.Vehicles.Consumers.Interfaces;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Producers.Interfaces;

namespace Rent.Vehicles.Consumers.Handlers.BackgroundServices;

public abstract class HandlerEventServicePublishBackgroundService<TEventToConsume> : HandlerEventPublishBackgroundService<TEventToConsume>
    where TEventToConsume : Messages.Event
{
    protected IServiceScopeFactory _serviceScopeFactory;

    protected HandlerEventServicePublishBackgroundService(ILogger<HandlerEventServicePublishBackgroundService<TEventToConsume>> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IServiceScopeFactory serviceScopeFactory) : base(logger, channel, periodicTimer, serializer, publisher)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }


}
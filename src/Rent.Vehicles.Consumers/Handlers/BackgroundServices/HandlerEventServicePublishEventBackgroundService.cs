using Rent.Vehicles.Consumers.Interfaces;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Messages;
using Rent.Vehicles.Producers.Interfaces;

namespace Rent.Vehicles.Consumers.Handlers.BackgroundServices;

public abstract class
    HandlerEventServicePublishEventBackgroundService<TEventToConsume> : HandlerEventPublishEventBackgroundService<
    TEventToConsume>
    where TEventToConsume : Event
{
    protected IServiceScopeFactory _serviceScopeFactory;

    protected HandlerEventServicePublishEventBackgroundService(
        ILogger<HandlerEventServicePublishEventBackgroundService<TEventToConsume>> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IServiceScopeFactory serviceProvider) : base(logger, channel, periodicTimer, serializer, publisher)
    {
        _serviceScopeFactory = serviceProvider;
    }
}

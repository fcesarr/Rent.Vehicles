
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib;
using Rent.Vehicles.Lib.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Messages;

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
        IOptions<ConsumerSetting> consumerSetting,
        IServiceScopeFactory serviceProvider) : base(logger, channel, periodicTimer, serializer, publisher, consumerSetting)
    {
        _serviceScopeFactory = serviceProvider;
    }
}

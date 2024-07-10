using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib;
using Rent.Vehicles.Lib.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;

namespace Rent.Vehicles.Consumers.Handlers.BackgroundServices;

public abstract class
    HandlerEventServicePublishBackgroundService<TEventToConsume> : HandlerEventPublishBackgroundService<TEventToConsume>
    where TEventToConsume : Event
{
    protected IServiceScopeFactory _serviceScopeFactory;

    protected HandlerEventServicePublishBackgroundService(
        ILogger<HandlerEventServicePublishBackgroundService<TEventToConsume>> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IOptions<ConsumerSetting> consumerSetting,
        IServiceScopeFactory serviceScopeFactory) : base(logger, channel, periodicTimer, serializer, publisher,
        consumerSetting)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }
}

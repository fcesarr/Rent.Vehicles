using Rent.Vehicles.Consumers.Types;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib;
using Rent.Vehicles.Lib.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;

namespace Rent.Vehicles.Consumers.Handlers.BackgroundServices;

public abstract class HandlerEventBackgroundService<TEventToConsume> : HandlerMessageBackgroundService<TEventToConsume>
    where TEventToConsume : Event
{
    protected HandlerEventBackgroundService(ILogger<HandlerEventBackgroundService<TEventToConsume>> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IOptions<ConsumerSetting> consumerSetting) : base(logger, channel, periodicTimer, serializer, consumerSetting)
    {
    }

    protected override ConsumerType _type => ConsumerType.Event;
}

using Rent.Vehicles.Consumers.Types;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib;
using Rent.Vehicles.Lib.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;

namespace Rent.Vehicles.Consumers.Handlers.BackgroundServices;

public abstract class
    HandlerCommandPublishBackgroundService<TCommandToConsume> : HandlerMessageBackgroundService<TCommandToConsume>
    where TCommandToConsume : Command
{
    protected readonly IPublisher _publisher;

    protected HandlerCommandPublishBackgroundService(
        ILogger<HandlerCommandPublishBackgroundService<TCommandToConsume>> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IOptions<ConsumerSetting> consumerSetting) : base(logger, channel, periodicTimer, serializer, consumerSetting)
    {
        _publisher = publisher;
    }

    protected override ConsumerType _type => ConsumerType.Command;
}

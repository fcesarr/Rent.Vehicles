using Rent.Vehicles.Consumers.Interfaces;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Messages;
using Rent.Vehicles.Producers.Interfaces;

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
        IPublisher publisher) : base(logger, channel, periodicTimer, serializer)
    {
        _publisher = publisher;
    }
}
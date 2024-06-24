using LanguageExt;
using LanguageExt.Common;

using RabbitMQ.Client;

using Rent.Vehicles.Consumers.Interfaces;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;

namespace Rent.Vehicles.Consumers.Handlers.BackgroundServices;

public abstract class HandlerEventBackgroundService<TEventToConsume> : HandlerMessageBackgroundService<TEventToConsume>
    where TEventToConsume : Messages.Event
{
    protected HandlerEventBackgroundService(ILogger<HandlerEventBackgroundService<TEventToConsume>> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer) : base(logger, channel, periodicTimer, serializer)
    {
    }    
}
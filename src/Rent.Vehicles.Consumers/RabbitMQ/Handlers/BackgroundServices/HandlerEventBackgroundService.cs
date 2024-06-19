using LanguageExt;
using LanguageExt.Common;

using RabbitMQ.Client;

using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;

namespace Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;

public abstract class HandlerEventBackgroundService<TEventToConsume> : HandlerMessageBackgroundService<TEventToConsume>
    where TEventToConsume : Messages.Event
{
    protected HandlerEventBackgroundService(ILogger<HandlerEventBackgroundService<TEventToConsume>> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer) : base(logger, channel, periodicTimer, serializer)
    {
    }

    protected override async Task<Result<Task>> HandlerAsync(TEventToConsume eventToPublish, CancellationToken cancellationToken = default)
        => await HandlerMessageAsync(eventToPublish, cancellationToken);

    protected abstract Task<Result<Task>> HandlerMessageAsync(TEventToConsume @event, CancellationToken cancellationToken = default);

    
}
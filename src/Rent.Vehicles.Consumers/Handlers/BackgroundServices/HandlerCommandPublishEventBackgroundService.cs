using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib;
using Rent.Vehicles.Lib.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services;

namespace Rent.Vehicles.Consumers.Handlers.BackgroundServices;

public abstract class
    HandlerCommandPublishEventBackgroundService<TCommandToConsume, TEventToPublish> :
    HandlerCommandPublishBackgroundService<TCommandToConsume>
    where TCommandToConsume : Command
    where TEventToPublish : Event
{
    protected IServiceScopeFactory _serviceScopeFactory;

    protected HandlerCommandPublishEventBackgroundService(
        ILogger<HandlerCommandPublishEventBackgroundService<TCommandToConsume, TEventToPublish>> logger,
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

    protected override async Task<Result<Task>> HandlerAsync(TCommandToConsume @event,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await base.HandlerAsync(@event, cancellationToken);

            if (!result.IsSuccess)
            {
                return result.Exception!;
            }

            await result.Value!;

            var eventsToPublish = CreateEventToPublish(@event);

            return PublishAsync(eventsToPublish, cancellationToken);
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    protected abstract TEventToPublish CreateEventToPublish(TCommandToConsume @event);

    protected virtual async Task PublishAsync(TEventToPublish @event, CancellationToken cancellationToken = default)
    {
        await _publisher.PublishSingleEventAsync(@event, cancellationToken);
    }
}

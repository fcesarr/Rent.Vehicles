using Rent.Vehicles.Consumers.Interfaces;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Messages.Types;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Services;

using Event = Rent.Vehicles.Messages.Events.Event;

namespace Rent.Vehicles.Consumers.Handlers.BackgroundServices;

public abstract class
    HandlerEventPublishBackgroundService<TEventToConsume> : HandlerEventBackgroundService<TEventToConsume>
    where TEventToConsume : Messages.Event
{
    protected readonly IPublisher _publisher;

    protected HandlerEventPublishBackgroundService(
        ILogger<HandlerEventPublishBackgroundService<TEventToConsume>> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher) : base(logger, channel, periodicTimer, serializer)
    {
        _publisher = publisher;
    }

    protected override async Task<Result<Task>> HandlerAsync(TEventToConsume eventToPublish,
        CancellationToken cancellationToken = default)
    {
        var @event = new Event
        {
            Id = Guid.NewGuid(),
            SagaId = eventToPublish.SagaId,
            Type = eventToPublish.GetType().ToString(),
            StatusType = StatusType.Success,
            Message = string.Empty
        };

        try
        {
            var result = await base.HandlerAsync(eventToPublish, cancellationToken);

            if (!result.IsSuccess)
            {
                await TreatException(@event, result.Exception!, cancellationToken).Value!;

                return result.Exception!;
            }

            await result.Value!;

            return _publisher.PublishSingleEventAsync(@event, cancellationToken);
        }
        catch (Exception exception)
        {
            return TreatException(@event, exception, cancellationToken).Value!;
        }
    }

    private Result<Task> TreatException(Event @event,
        Exception exception,
        CancellationToken cancellationToken = default)
    {
        return _publisher.PublishSingleEventAsync(
            @event with { StatusType = StatusType.Fail, Message = exception.Message }, cancellationToken);
    }
}

using RabbitMQ.Client;

using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Producers.Interfaces;

using Event = Rent.Vehicles.Messages.Events.Event;

namespace Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;

public abstract class HandlerEventPublishBackgroundService<TEventToConsume> : HandlerEventBackgroundService<TEventToConsume>
    where TEventToConsume : Messages.Event
{
    protected readonly IPublisher _publisher;

    protected HandlerEventPublishBackgroundService(ILogger<HandlerEventPublishBackgroundService<TEventToConsume>> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher) : base(logger, channel, periodicTimer, serializer)
    {
        _publisher = publisher;
    }

    protected override async Task HandlerAsync(TEventToConsume eventToPublish, CancellationToken cancellationToken = default)
    {
        var @event = new Event
        {
            SagaId = eventToPublish.SagaId,
            Name = typeof(TEventToConsume).Name,
            StatusType = Entities.StatusType.Success,
            Message = string.Empty
        };

        try
        {
            await base.HandlerAsync(eventToPublish, cancellationToken);

            await _publisher.PublishSingleEventAsync(@event, cancellationToken);
        }
        catch (Exception ex)
        {   
            @event = @event with { StatusType = Entities.StatusType.Fail, Message = ex.Message };

            await _publisher.PublishSingleEventAsync(@event, cancellationToken);

            _logger.LogError(ex, ex.Message);

            throw;
        }
    }
}
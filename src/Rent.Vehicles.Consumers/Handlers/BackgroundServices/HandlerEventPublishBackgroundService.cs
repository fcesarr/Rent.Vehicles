
using LanguageExt;
using LanguageExt.Common;

using RabbitMQ.Client;

using Rent.Vehicles.Consumers.Interfaces;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Entities.Types;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Producers.Interfaces;

using Event = Rent.Vehicles.Messages.Events.Event;

namespace Rent.Vehicles.Consumers.Handlers.BackgroundServices;

public abstract class HandlerEventPublishBackgroundService<TEventToConsume> : HandlerEventBackgroundService<TEventToConsume>
    where TEventToConsume : Messages.Event
{
    protected readonly IPublisher _publisher;

    protected HandlerEventPublishBackgroundService(ILogger<HandlerEventPublishBackgroundService<TEventToConsume>> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher) : base(logger, channel, periodicTimer, serializer)
    {
        _publisher = publisher;
    }

    protected override async Task<Result<Task>> HandlerAsync(TEventToConsume eventToPublish, CancellationToken cancellationToken = default)
    {
        var @event = new Event
        {
            SagaId = eventToPublish.SagaId,
            Type = typeof(TEventToConsume).Name,
            StatusType = Messages.Types.StatusType.Success,
            Message = string.Empty
        };

        try
        {
            var result = await base.HandlerAsync(eventToPublish, cancellationToken);
    
            return result.Match(result => {
                result
                    .GetAwaiter()
                    .GetResult();
    
                _publisher.PublishSingleEventAsync(@event, cancellationToken)
                    .GetAwaiter()
                    .GetResult();
    
                return Task.CompletedTask;
            }, exception => TreatException(@event, exception, cancellationToken));
        }
        catch (Exception ex)
        {
            return TreatException(@event, ex, cancellationToken);
        }
    }

    private async Task<Result<Task>> TreatException(Event @event,
        Exception exception,
        CancellationToken cancellationToken = default)
    {
        @event = @event with { StatusType = Messages.Types.StatusType.Fail, Message = exception.Message };

        await _publisher.PublishSingleEventAsync(@event, cancellationToken);

        return Task.FromResult(new Result<Task>(new Exception()));
    }
}
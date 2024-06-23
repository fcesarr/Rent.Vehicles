
using LanguageExt;
using LanguageExt.Common;

using RabbitMQ.Client;

using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Entities.Types;
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

    protected override async Task<Result<Task>> HandlerAsync(TEventToConsume eventToPublish, CancellationToken cancellationToken = default)
    {
        var @event = new Event
        {
            SagaId = eventToPublish.SagaId,
            Name = typeof(TEventToConsume).Name,
            StatusType = Messages.Types.StatusType.Success,
            Message = string.Empty
        };

        var result = await base.HandlerAsync(eventToPublish, cancellationToken);

        return result.Match(result => {
            result
                .GetAwaiter()
                .GetResult();

            _publisher.PublishSingleEventAsync(@event, cancellationToken)
                .GetAwaiter()
                .GetResult();

            return Task.CompletedTask;
        }, exception => {
             @event = @event with { StatusType = Messages.Types.StatusType.Fail, Message = exception.Message };

            _publisher.PublishSingleEventAsync(@event, cancellationToken)
                .GetAwaiter()
                .GetResult();

            return new Result<Task>(exception);
        });
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
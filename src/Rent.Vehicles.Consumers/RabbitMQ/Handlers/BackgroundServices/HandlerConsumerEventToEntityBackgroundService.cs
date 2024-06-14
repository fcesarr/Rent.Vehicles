
using RabbitMQ.Client;

using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;

namespace Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;

public abstract class HandlerConsumerEventToEntityBackgroundService<TEvent, TEntity> : HandlerConsumerMessageBackgroundService<TEvent> 
    where TEvent : Messages.Event
    where TEntity : class
{
    private readonly IBothServices<Entities.Event> _createEventService;

    protected HandlerConsumerEventToEntityBackgroundService(ILogger<HandlerConsumerEventToEntityBackgroundService<TEvent, TEntity>> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        string queueName,
        IBothServices<Entities.Event> createEventService) : base(logger, channel, periodicTimer, serializer, queueName)
    {
        _createEventService = createEventService;
    }

    protected abstract Task<TEntity> EventToEntityAsync(TEvent @event, CancellationToken cancellationToken = default);

    protected override async Task HandlerAsync(TEvent @event, CancellationToken cancellationToken = default)
    {
        var entity = await EventToEntityAsync(@event, cancellationToken);

        try
        {
            await HandlerAsync(entity, cancellationToken);
        }
        catch (Exception ex)
        {
            await _createEventService.CreateAsync(new Entities.Event{
                SagaId = @event.SagaId,
                Name = typeof(TEvent).Name,
                StatusType = Entities.StatusType.Fail,
                Message = ex.Message
            }, cancellationToken);
        }

        await _createEventService.CreateAsync(new Entities.Event{
                SagaId = @event.SagaId,
                Name = typeof(TEvent).Name,
                StatusType = Entities.StatusType.Success,
                Message = string.Empty
            }, cancellationToken);
    }

    protected abstract Task HandlerAsync(TEntity entity, CancellationToken cancellationToken = default);
}
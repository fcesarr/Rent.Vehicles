
using RabbitMQ.Client;

using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Entities;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;

namespace Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;

public abstract class HandlerConsumerEventBackgroundService<TEvent> : HandlerConsumerMessageBackgroundService<TEvent> 
    where TEvent : Messages.Event
{
    private readonly ICreateService<Event> _createService;

    protected HandlerConsumerEventBackgroundService(ILogger<HandlerConsumerEventBackgroundService<TEvent>> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        ICreateService<Event> createEventService) : base(logger, channel, periodicTimer, serializer)
    {
        _createService = createEventService;
    }

    protected override async Task HandlerAsync(TEvent @event, CancellationToken cancellationToken = default)
    {
        try
        {
            await HandlerEventAsync(@event, cancellationToken);
        }
        catch (Exception ex)
        {
            await _createService.CreateAsync(new Event{
                SagaId = @event.SagaId,
                Name = typeof(TEvent).Name,
                StatusType = StatusType.Fail,
                Message = ex.Message
            }, cancellationToken);
        }

        await _createService.CreateAsync(new Event{
                SagaId = @event.SagaId,
                Name = typeof(TEvent).Name,
                StatusType = StatusType.Success,
                Message = string.Empty
            }, cancellationToken);
    }

    protected abstract Task HandlerEventAsync(TEvent @event, CancellationToken cancellationToken = default);
}
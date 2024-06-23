
using LanguageExt.Common;

using RabbitMQ.Client;

using Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Services.Interfaces;

using EventEntity = Rent.Vehicles.Entities.Event;
using Event = Rent.Vehicles.Messages.Events.Event;


namespace Rent.Vehicles.Consumers.RabbitMQ.Events.BackgroundServices;

public class EventBackgroundService : HandlerEventBackgroundService<Event>
{
    private readonly IService<EventEntity> _service;

    public EventBackgroundService(ILogger<EventBackgroundService> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IService<EventEntity> service) : base(logger, channel, periodicTimer, serializer)
    {
        _service = service;
    }

    protected override async Task<Result<Task>> HandlerMessageAsync(Event @event, CancellationToken cancellationToken = default)
    {
        var entity = await _service.CreateAsync(new EventEntity
        {
            SagaId = @event.SagaId,
            Name = @event.Name,
            StatusType = @event.StatusType switch {
                Messages.Types.StatusType.Success => Entities.Types.StatusType.Success,
                Messages.Types.StatusType.Fail or _ => Entities.Types.StatusType.Fail
            },
            Message = @event.Message,
        }, cancellationToken);

        return entity.Match(entity => Task.CompletedTask, exception => new Result<Task>(exception));
    }
}

using RabbitMQ.Client;

using Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Entities;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Messages;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Services.Interfaces;

using Event = Rent.Vehicles.Messages.Events.Event;


namespace Rent.Vehicles.Consumers.RabbitMQ.Events.BackgroundServices;

public class EventBackgroundService : HandlerEventBackgroundService<Event>
{
    private readonly INoSqlService<Entities.Event> _service;

    public EventBackgroundService(ILogger<EventBackgroundService> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        INoSqlService<Entities.Event> service) : base(logger, channel, periodicTimer, serializer)
    {
        _service = service;
    }

    protected override async Task HandlerMessageAsync(Event @event, CancellationToken cancellationToken = default)
    {
        await _service.CreateAsync(new Entities.Event
        {
            SagaId = @event.SagaId,
            Name = @event.Name,
            StatusType = @event.StatusType,
            Message = @event.Message,
        }, cancellationToken);
    }
}
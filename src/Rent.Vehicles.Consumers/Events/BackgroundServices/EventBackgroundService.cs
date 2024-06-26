
using Rent.Vehicles.Consumers.Handlers.BackgroundServices;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Services.Interfaces;

using EventEntity = Rent.Vehicles.Entities.Event;
using Event = Rent.Vehicles.Messages.Events.Event;
using Rent.Vehicles.Consumers.Interfaces;
using Rent.Vehicles.Services;


namespace Rent.Vehicles.Consumers.Events.BackgroundServices;

public class EventBackgroundService : HandlerEventBackgroundService<Event>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public EventBackgroundService(ILogger<EventBackgroundService> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IServiceScopeFactory serviceScopeFactory) : base(logger, channel, periodicTimer, serializer)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task<Result<Task>> HandlerMessageAsync(Event @event, CancellationToken cancellationToken = default)
    {
        var _service = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IDataService<EventEntity>>();

        var entity = await _service.CreateAsync(new EventEntity
        {
            SagaId = @event.SagaId,
            Name = @event.Type,
            StatusType = @event.StatusType switch {
                Messages.Types.StatusType.Success => Entities.Types.StatusType.Success,
                Messages.Types.StatusType.Fail or _ => Entities.Types.StatusType.Fail
            },
            Message = @event.Message,
            SerializerType = Lib.Types.SerializerType.MessagePack,
            Data = await _serializer.SerializeAsync(@event, cancellationToken)
        }, cancellationToken);

        if(!entity.IsSuccess)
            return entity.Exception!;

        return Task.CompletedTask;
    }
}
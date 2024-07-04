using Rent.Vehicles.Consumers.Handlers.BackgroundServices;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Messages.Projections.Events;
using Rent.Vehicles.Services;
using Rent.Vehicles.Services.Facades.Interfaces;

using Event = Rent.Vehicles.Messages.Events.Event;


namespace Rent.Vehicles.Consumers.Events.BackgroundServices;

public class EventBackgroundService : HandlerEventBackgroundService<Event>
{
    private readonly IPublisher _publisher;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public EventBackgroundService(ILogger<EventBackgroundService> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IServiceScopeFactory serviceScopeFactory) : base(logger, channel, periodicTimer, serializer)
    {
        _publisher = publisher;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task<Result<Task>> HandlerMessageAsync(Event @event,
        CancellationToken cancellationToken = default)
    {
        using var serviceScope = _serviceScopeFactory.CreateScope();

        var serviceProvider = serviceScope.ServiceProvider;

        var service = serviceProvider.GetRequiredService<IEventFacade>();

        var entity = await service.CreateAsync(@event, cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!;
        }

        return _publisher.PublishSingleEventAsync(new EventProjectionEvent{ Id = @event.Id, SagaId = @event.SagaId }, cancellationToken);
    }
}

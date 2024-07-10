using Rent.Vehicles.Consumers.Handlers.BackgroundServices;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Messages.Projections.Events;
using Rent.Vehicles.Services;
using Rent.Vehicles.Services.Facades.Interfaces;

namespace Rent.Vehicles.Consumers.Events.BackgroundServices;

public class EventProjectionEventBackgroundService : HandlerEventBackgroundService<EventProjectionEvent>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public EventProjectionEventBackgroundService(ILogger<EventProjectionEventBackgroundService> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IServiceScopeFactory serviceScopeFactory,
        IOptions<ConsumerSetting> consumerSetting) : base(logger, channel, periodicTimer, serializer, consumerSetting)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task<Result<Task>> HandlerMessageAsync(EventProjectionEvent @event,
        CancellationToken cancellationToken = default)
    {
        using var serviceScope = _serviceScopeFactory.CreateScope();

        var serviceProvider = serviceScope.ServiceProvider;

        var service = serviceProvider.GetRequiredService<IEventProjectionFacade>();

        var entity = await service.CreateAsync(@event, cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!;
        }

        return Task.CompletedTask;
    }
}

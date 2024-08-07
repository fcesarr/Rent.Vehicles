using Rent.Vehicles.Consumers.Handlers.BackgroundServices;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Messages.Projections.Events;
using Rent.Vehicles.Services;
using Rent.Vehicles.Services.Facades.Interfaces;

using Event = Rent.Vehicles.Lib.Event;

namespace Rent.Vehicles.Consumers.Events.BackgroundServices;

public class CreateUserEventBackgroundService : HandlerEventServicePublishEventBackgroundService<
    CreateUserEvent>
{
    public CreateUserEventBackgroundService(ILogger<CreateUserEventBackgroundService> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IOptions<ConsumerSetting> consumerSetting,
        IServiceScopeFactory serviceScopeFactory) : base(logger, channel, periodicTimer, serializer, publisher,
        consumerSetting, serviceScopeFactory)
    {
    }

    protected override IEnumerable<Event> CreateEventToPublish(CreateUserEvent @event)
    {
        return
        [
            new CreateUserProjectionEvent { Id = @event.Id, SagaId = @event.SagaId },
            new UploadUserLicenseImageEvent
            {
                Id = @event.Id, LicenseImage = @event.LicenseImage, SagaId = @event.SagaId
            }
        ];
    }

    protected override async Task<Result<Task>> HandlerMessageAsync(CreateUserEvent @event,
        CancellationToken cancellationToken = default)
    {
        using var serviceScope = _serviceScopeFactory.CreateScope();

        var serviceProvider = serviceScope.ServiceProvider;

        var service = serviceProvider.GetRequiredService<IUserFacade>();

        var entity = await service.CreateAsync(@event, cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!;
        }

        return Task.CompletedTask;
    }
}

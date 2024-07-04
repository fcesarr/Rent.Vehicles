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

public class UpdateUserLicenseImageEventBackgroundService : HandlerEventServicePublishEventBackgroundService<
    UpdateUserLicenseImageEvent>
{
    public UpdateUserLicenseImageEventBackgroundService(ILogger<UpdateUserLicenseImageEventBackgroundService> logger,
        IConsumer channel, IPeriodicTimer periodicTimer, ISerializer serializer, IPublisher publisher,
        IServiceScopeFactory serviceScopeFactory) : base(logger, channel, periodicTimer, serializer, publisher,
        serviceScopeFactory)
    {
    }

    protected override IEnumerable<Event> CreateEventToPublish(UpdateUserLicenseImageEvent @event)
    {
        return
        [
            new UpdateUserProjectionEvent { Id = @event.Id, SagaId = @event.SagaId },
            new UploadUserLicenseImageEvent
            {
                Id = @event.Id, LicenseImage = @event.LicenseImage, SagaId = @event.SagaId
            }
        ];
    }

    protected override async Task<Result<Task>> HandlerMessageAsync(UpdateUserLicenseImageEvent @event,
        CancellationToken cancellationToken = default)
    {
        using var serviceScope = _serviceScopeFactory.CreateScope();

        var serviceProvider = serviceScope.ServiceProvider;

        var service = serviceProvider.GetRequiredService<IUserFacade>();

        var result = await service.UpdateAsync(@event, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Exception!;
        }

        return Task.CompletedTask;
    }
}

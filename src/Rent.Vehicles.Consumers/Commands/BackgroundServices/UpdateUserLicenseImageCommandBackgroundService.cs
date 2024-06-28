using Rent.Vehicles.Consumers.Handlers.BackgroundServices;
using Rent.Vehicles.Consumers.Interfaces;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Entities.Types;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Lib.Types;
using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Services;
using Rent.Vehicles.Services.Facades.Interfaces;

namespace Rent.Vehicles.Consumers.Commands.BackgroundServices;

public class UpdateUserLicenseImageCommandBackgroundService : HandlerCommandPublishEventBackgroundService<
    UpdateUserLicenseImageCommand,
    UpdateUserLicenseImageEvent>
{
    public UpdateUserLicenseImageCommandBackgroundService(
        ILogger<UpdateUserLicenseImageCommandBackgroundService> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IServiceScopeFactory serviceScopeFactory) : base(logger, channel, periodicTimer, serializer, publisher,
        serviceScopeFactory)
    {
    }

    protected override UpdateUserLicenseImageEvent CreateEventToPublish(UpdateUserLicenseImageCommand command)
    {
        return new UpdateUserLicenseImageEvent
        {
            Id = command.Id, LicenseImage = command.LicenseImage, SagaId = command.SagaId
        };
    }

    protected override async Task<Result<Task>> HandlerMessageAsync(UpdateUserLicenseImageCommand command,
        CancellationToken cancellationToken = default)
    {
        var service = _serviceScopeFactory.CreateScope()
            .ServiceProvider
            .GetRequiredService<ICommandFacade>();

        var @event = CreateEventToPublish(command);

        var entity = await service.CreateAsync(command, @event, ActionType.Update,
            EntityType.User, nameof(UpdateUserLicenseImageEvent), cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!;
        }

        return Task.CompletedTask;
    }
}

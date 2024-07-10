using Rent.Vehicles.Consumers.Handlers.BackgroundServices;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Entities.Types;
using Rent.Vehicles.Lib.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Services;
using Rent.Vehicles.Services.Facades.Interfaces;

namespace Rent.Vehicles.Consumers.Commands.BackgroundServices;

public class UpdateUserCommandBackgroundService : HandlerCommandPublishEventBackgroundService<
    UpdateUserCommand,
    UpdateUserEvent>
{
    public UpdateUserCommandBackgroundService(
        ILogger<HandlerCommandPublishEventBackgroundService<UpdateUserCommand, UpdateUserEvent>> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IOptions<ConsumerSetting> consumerSetting,
        IServiceScopeFactory serviceScopeFactory) : base(logger, channel, periodicTimer, serializer, publisher,
        consumerSetting, serviceScopeFactory)
    {
    }

    protected override UpdateUserEvent CreateEventToPublish(UpdateUserCommand @event)
    {
        return new UpdateUserEvent
        {
            Id = @event.Id,
            Name = @event.Name,
            Number = @event.Number,
            Birthday = @event.Birthday,
            LicenseNumber = @event.LicenseNumber,
            LicenseType = @event.LicenseType,
            SagaId = @event.SagaId
        };
    }

    protected override async Task<Result<Task>> HandlerMessageAsync(UpdateUserCommand command,
        CancellationToken cancellationToken = default)
    {
        using var serviceScope = _serviceScopeFactory.CreateScope();

        var serviceProvider = serviceScope.ServiceProvider;

        var service = serviceProvider.GetRequiredService<ICommandFacade>();

        var @event = CreateEventToPublish(command);

        var entity = await service.CreateAsync(command, @event, ActionType.Update,
            EntityType.User, @event.GetType().ToString(), cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!;
        }

        return Task.CompletedTask;
    }
}

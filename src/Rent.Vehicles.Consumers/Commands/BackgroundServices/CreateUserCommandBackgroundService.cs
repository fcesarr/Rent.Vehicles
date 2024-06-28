using Rent.Vehicles.Consumers.Handlers.BackgroundServices;
using Rent.Vehicles.Consumers.Interfaces;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Entities;
using Rent.Vehicles.Entities.Types;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Lib.Types;
using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Services;
using Rent.Vehicles.Services.DataServices.Interfaces;
using Rent.Vehicles.Services.Facades.Interfaces;

namespace Rent.Vehicles.Consumers.Commands.BackgroundServices;

public class CreateUserCommandBackgroundService : HandlerCommandPublishEventBackgroundService<
    CreateUserCommand,
    CreateUserEvent>
{
    public CreateUserCommandBackgroundService(ILogger<CreateUserCommandBackgroundService> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IServiceScopeFactory serviceScopeFactory) : base(logger, channel, periodicTimer, serializer, publisher,
        serviceScopeFactory)
    {
    }

    protected override CreateUserEvent CreateEventToPublish(CreateUserCommand command)
    {
        return new CreateUserEvent
        {
            Id = command.Id,
            Name = command.Name,
            Number = command.Number,
            Birthday = command.Birthday,
            LicenseNumber = command.LicenseNumber,
            LicenseImage = command.LicenseImage,
            SagaId = command.SagaId
        };
    }

    protected override async Task<Result<Task>> HandlerMessageAsync(CreateUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var service = _serviceScopeFactory.CreateScope()
            .ServiceProvider
            .GetRequiredService<ICommandFacade>();

        var @event = CreateEventToPublish(command);

        var entity = await service.CreateAsync(command, @event, ActionType.Create,
            EntityType.User, nameof(CreateUserEvent), cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!;
        }

        return Task.CompletedTask;
    }
}

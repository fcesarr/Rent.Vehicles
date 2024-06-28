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

public class CreateRentCommandBackgroundService : HandlerCommandPublishEventBackgroundService<
    CreateRentCommand,
    CreateRentEvent>
{
    public CreateRentCommandBackgroundService(ILogger<CreateRentCommandBackgroundService> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IServiceScopeFactory serviceScopeFactory) : base(logger, channel, periodicTimer, serializer, publisher,
        serviceScopeFactory)
    {
    }

    protected override CreateRentEvent CreateEventToPublish(CreateRentCommand command)
    {
        return new CreateRentEvent
        {
            Id = command.Id, UserId = command.UserId, RentPlaneId = command.RentPlaneId, SagaId = command.SagaId
        };
    }

    protected override async Task<Result<Task>> HandlerMessageAsync(CreateRentCommand command,
        CancellationToken cancellationToken = default)
    {
        var service = _serviceScopeFactory.CreateScope()
            .ServiceProvider
            .GetRequiredService<ICommandFacade>();

        var @event = CreateEventToPublish(command);

        var entity = await service.CreateAsync(command, @event, ActionType.Create,
            EntityType.Rent, nameof(CreateRentEvent), cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!;
        }

        return Task.CompletedTask;
    }
}

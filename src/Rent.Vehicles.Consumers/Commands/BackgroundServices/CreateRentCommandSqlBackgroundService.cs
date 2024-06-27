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

namespace Rent.Vehicles.Consumers.Commands.BackgroundServices;

public class CreateRentCommandSqlBackgroundService : HandlerCommandPublishEventBackgroundService<
    CreateRentCommand,
    CreateRentEvent>
{
    public CreateRentCommandSqlBackgroundService(ILogger<CreateRentCommandSqlBackgroundService> logger,
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
        ICommandDataService service = _serviceScopeFactory.CreateScope()
            .ServiceProvider
            .GetRequiredService<ICommandDataService>();

        Command entity = new()
        {
            SagaId = command.SagaId,
            ActionType = ActionType.Create,
            SerializerType = SerializerType.MessagePack,
            EntityType = EntityType.Rent,
            Type = typeof(CreateRentEvent).Name,
            Data = await _serializer.SerializeAsync(CreateEventToPublish(command), cancellationToken)
        };

        return service.CreateAsync(entity, cancellationToken);
    }
}
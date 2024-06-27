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

public class DeleteVehiclesCommandSqlBackgroundService : HandlerCommandPublishEventBackgroundService<
    DeleteVehiclesCommand,
    DeleteVehiclesEvent>
{
    public DeleteVehiclesCommandSqlBackgroundService(ILogger<DeleteVehiclesCommandSqlBackgroundService> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IServiceScopeFactory serviceScopeFactory) : base(logger, channel, periodicTimer, serializer, publisher,
        serviceScopeFactory)
    {
    }

    protected override DeleteVehiclesEvent CreateEventToPublish(DeleteVehiclesCommand command)
    {
        return new DeleteVehiclesEvent { Id = command.Id, SagaId = command.SagaId };
    }

    protected override async Task<Result<Task>> HandlerMessageAsync(DeleteVehiclesCommand command,
        CancellationToken cancellationToken = default)
    {
        var service = _serviceScopeFactory.CreateScope()
            .ServiceProvider
            .GetRequiredService<ICommandDataService>();

        Command entity = new()
        {
            SagaId = command.SagaId,
            ActionType = ActionType.Delete,
            SerializerType = SerializerType.MessagePack,
            EntityType = EntityType.Vehicles,
            Type = nameof(DeleteVehiclesEvent),
            Data = await _serializer.SerializeAsync(CreateEventToPublish(command), cancellationToken)
        };

        return service.CreateAsync(entity, cancellationToken);
    }
}

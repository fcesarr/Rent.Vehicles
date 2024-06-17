using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Entities;
using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;

namespace Rent.Vehicles.Consumers.RabbitMQ.Commands.BackgroundServices;

public class DeleteVehiclesCommandSqlBackgroundService : HandlerServiceMessageAndPublisherBackgroundService<
    DeleteVehiclesCommand, 
    DeleteVehiclesEvent,
    Command,
    ISqlService<Command>>
{
    public DeleteVehiclesCommandSqlBackgroundService(ILogger<DeleteVehiclesCommandSqlBackgroundService> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        ISqlService<Command> service) : base(logger, channel, periodicTimer, serializer, publisher, service)
    {
    }

    protected override DeleteVehiclesEvent CommandToEvent(DeleteVehiclesCommand command)
    {
        return new DeleteVehiclesEvent
        {
            Id = command.Id,
            SagaId = command.SagaId
        };
    }

    protected override async Task HandlerMessageAsync(DeleteVehiclesCommand command,
        CancellationToken cancellationToken = default)
    {
        var entity = new Command
        {
            SagaId = command.SagaId,
            ActionType = Entities.Types.ActionType.Delete,
            SerializerType = Lib.Types.SerializerType.MessagePack,
            EntityType = Entities.Types.EntityType.Vehicles,
            Type = typeof(DeleteVehiclesEvent).Name,
            Data = await _serializer.SerializeAsync(CommandToEvent(command))
        };

        await _service.CreateAsync(entity, cancellationToken);
    }
}
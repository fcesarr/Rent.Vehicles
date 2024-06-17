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

public class UpdateVehiclesCommandSqlBackgroundService : HandlerMessageAndActionAndPublisherBackgroundService<
    UpdateVehiclesCommand,
    UpdateVehiclesEvent,
    Command,
    ISqlService<Command>>
{
    public UpdateVehiclesCommandSqlBackgroundService(ILogger<UpdateVehiclesCommandSqlBackgroundService> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        ISqlService<Command> service) : base(logger, channel, periodicTimer, serializer, publisher, service)
    {
    }

    protected override UpdateVehiclesEvent CommandToEvent(UpdateVehiclesCommand command)
    {
        return new UpdateVehiclesEvent
        {
            Id = command.Id, 
            LicensePlate = command.LicensePlate,
            SagaId = command.SagaId
        };
    }

    protected override async Task HandlerMessageAsync(UpdateVehiclesCommand command, CancellationToken cancellationToken = default)
    {
        var entity = new Command
        {
            SagaId = command.SagaId,
            ActionType = Entities.Types.ActionType.Update,
            SerializerType = Lib.Types.SerializerType.MessagePack,
            EntityType = Entities.Types.EntityType.Vehicles,
            Type = typeof(DeleteVehiclesEvent).Name,
            Data = await _serializer.SerializeAsync(CommandToEvent(command))
        };

        await _service.CreateAsync(entity, cancellationToken);
    }
}
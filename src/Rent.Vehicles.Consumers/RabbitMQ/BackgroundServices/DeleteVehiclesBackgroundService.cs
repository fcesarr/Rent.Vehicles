using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Entities;
using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Consumers.RabbitMQ.BackgroundServices.Abstracts;

namespace Rent.Vehicles.Consumers.RabbitMQ.BackgroundServices;

public class DeleteVehiclesBackgroundService : DeleteBackgroundService<DeleteVehiclesCommand, Command>
{
    public DeleteVehiclesBackgroundService(ILogger<DeleteVehiclesBackgroundService> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IDeleteService<Command> deleteService) : base(logger, channel, periodicTimer, serializer, deleteService)
    {
    }

    protected override async Task<Command> CommandToEntity(DeleteVehiclesCommand message, ISerializer serializer)
    {
        return new Command
        {
            SagaId = message.SagaId,
            ActionType = Entities.Types.ActionType.Create,
            SerializerType = Lib.Types.SerializerType.MessagePack,
            EntityType = Entities.Types.EntityType.Vehicles,
            Data = await serializer.SerializeAsync(new { 
                Id = message.Id
            })
        };
    }

    protected override async Task Handler(Command entity, CancellationToken cancellationToken = default)
    {
        await _deleteService.DeleteAsync(entity, cancellationToken);
    }
}
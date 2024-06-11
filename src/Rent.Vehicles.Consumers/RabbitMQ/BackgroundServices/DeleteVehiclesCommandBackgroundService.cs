using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Entities;
using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Consumers.RabbitMQ.BackgroundServices.Abstracts;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Producers.Interfaces;

namespace Rent.Vehicles.Consumers.RabbitMQ.BackgroundServices;

public class DeleteVehiclesCommandBackgroundService : ConsumerDeleteCommandBackgroundService<DeleteVehiclesCommand, DeleteVehiclesEvent, Command>
{
    public DeleteVehiclesCommandBackgroundService(ILogger<DeleteVehiclesCommandBackgroundService> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IDeleteService<Command> deleteService) : base(logger, channel, periodicTimer, serializer, publisher, deleteService)
    {
    }

    protected override async Task<Command> CommandToEntityAsync(DeleteVehiclesCommand message,
        ISerializer serializer,
        CancellationToken cancellationToken = default)
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

    protected override async Task<DeleteVehiclesEvent> CommandToEventAsync(DeleteVehiclesCommand message, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() => {
            return new DeleteVehiclesEvent
            {
                Id = message.Id,
                SagaId = message.SagaId
            };
        }, cancellationToken);
    }

    protected override async Task HandlerAsync(Command entity, CancellationToken cancellationToken = default)
    {
        await _deleteService.DeleteAsync(entity, cancellationToken);
    }
}
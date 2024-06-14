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

public class DeleteVehiclesCommandBackgroundService : HandlerConsumerCommandPublisherBackgroundService<DeleteVehiclesCommand, DeleteVehiclesEvent>
{
    public DeleteVehiclesCommandBackgroundService(ILogger<DeleteVehiclesCommandBackgroundService> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        ICreateService<Command> createService) : base(logger, channel, periodicTimer, serializer, publisher, createService)
    {
    }

    protected override async Task<Command> CommandToEntityAsync(DeleteVehiclesCommand message,
        ISerializer serializer,
        CancellationToken cancellationToken = default)
    {
        return new Command
        {
            SagaId = message.SagaId,
            ActionType = Entities.Types.ActionType.Delete,
            SerializerType = Lib.Types.SerializerType.MessagePack,
            EntityType = Entities.Types.EntityType.Vehicles,
            Type = typeof(CreateVehiclesEvent).Name,
            Data = await serializer.SerializeAsync(new DeleteVehiclesEvent{ 
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
}
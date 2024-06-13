using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Entities;
using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Consumers.RabbitMQ.Consumers.BackgroundServices;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Producers.Interfaces;

namespace Rent.Vehicles.Consumers.RabbitMQ.Commands.BackgroundServices;

public class UpdateVehiclesCommandBackgroundService : ConsumerCreateCommandPublisherBackgroundService<UpdateVehiclesCommand, UpdateVehiclesEvent, Command>
{
    public UpdateVehiclesCommandBackgroundService(ILogger<UpdateVehiclesCommandBackgroundService> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        ICreateService<Command> createService) : base(logger, channel, periodicTimer, serializer, "UpdateVehiclesCommand", publisher, createService)
    {
    }

    protected override async Task<Command> CommandToEntityAsync(UpdateVehiclesCommand message,
        ISerializer serializer,
        CancellationToken cancellationToken = default)
    {
        return new Command
        {
            SagaId = message.SagaId,
            ActionType = Entities.Types.ActionType.Update,
            SerializerType = Lib.Types.SerializerType.MessagePack,
            EntityType = Entities.Types.EntityType.Vehicles,
            Data = await serializer.SerializeAsync(new { 
                Id = message.Id,
                LicensePlate = message.LicensePlate
            })
        };
    }

    protected override async Task<UpdateVehiclesEvent> CommandToEventAsync(UpdateVehiclesCommand message, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() => {
            return new UpdateVehiclesEvent
            {
                Id = message.Id,
                LicensePlate = message.LicensePlate,
                SagaId = message.SagaId
            };
        }, cancellationToken);
    }
}
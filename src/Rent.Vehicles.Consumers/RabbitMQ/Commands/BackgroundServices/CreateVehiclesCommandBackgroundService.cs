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

public class CreateVehiclesCommandBackgroundService : ConsumerCreateCommandBackgroundService<CreateVehiclesCommand, CreateVehiclesEvent, Command>
{
    public CreateVehiclesCommandBackgroundService(ILogger<CreateVehiclesCommandBackgroundService> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        ICreateService<Command> createService) : base(logger, channel, periodicTimer, serializer, "CreateVehiclesCommand", publisher, createService)
    {
    }

    protected override async Task<Command> CommandToEntityAsync(CreateVehiclesCommand message,
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
                Id = message.Id, 
                Year = message.Year,
                Model = message.Model,
                LicensePlate = message.LicensePlate,
                Type = message.Type
            })
        };
    }

    protected override async Task<CreateVehiclesEvent> CommandToEventAsync(CreateVehiclesCommand message, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() => {
            return new CreateVehiclesEvent
            {
                Id = message.Id, 
                Year = message.Year,
                Model = message.Model,
                LicensePlate = message.LicensePlate,
                Type = message.Type,
                SagaId = message.SagaId
            };
        }, cancellationToken);
    }
}
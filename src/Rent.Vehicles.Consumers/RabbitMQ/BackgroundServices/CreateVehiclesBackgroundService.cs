using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Entities;
using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Consumers.RabbitMQ.BackgroundServices.Abstracts;

namespace Rent.Vehicles.Consumers.RabbitMQ.BackgroundServices;

public class CreateVehiclesBackgroundService : CreateBackgroundService<CreateVehiclesCommand, Command>
{
    public CreateVehiclesBackgroundService(ILogger<CreateVehiclesBackgroundService> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        ICreateService<Command> createService) : base(logger, channel, periodicTimer, serializer, createService)
    {
    }

    protected override async Task<Command> CommandToEntity(CreateVehiclesCommand message, ISerializer serializer)
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

    protected override async Task Handler(Command entity, CancellationToken cancellationToken = default)
    {
        await _createService.CreateAsync(entity, cancellationToken);
    }
}
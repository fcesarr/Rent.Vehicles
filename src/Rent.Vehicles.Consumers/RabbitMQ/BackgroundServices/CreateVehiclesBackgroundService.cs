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

public class CreateVehiclesBackgroundService : CreateBackgroundService<CreateVehiclesCommand, CreateVehiclesEvent, Command>
{
    public CreateVehiclesBackgroundService(ILogger<CreateVehiclesBackgroundService> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        ICreateService<Command> createService) : base(logger, channel, periodicTimer, serializer, publisher, createService)
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

    protected override async Task HandlerAsync(Command entity, CancellationToken cancellationToken = default)
    {
        await _createService.CreateAsync(entity, cancellationToken);
    }
}
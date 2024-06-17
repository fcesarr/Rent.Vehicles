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

public class CreateVehiclesCommandSqlBackgroundService : HandlerCommandServicePublishEventBackgroundService<
    CreateVehiclesCommand,
    CreateVehiclesEvent,
    Command,
    ISqlService<Command>>
{
    public CreateVehiclesCommandSqlBackgroundService(ILogger<CreateVehiclesCommandSqlBackgroundService> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        ISqlService<Command> service) : base(logger, channel, periodicTimer, serializer, publisher, service)
    {
    }

    protected override CreateVehiclesEvent CreateEventToPublish(CreateVehiclesCommand command)
    {
        return new CreateVehiclesEvent
        {
            Id = command.Id, 
            Year = command.Year,
            Model = command.Model,
            LicensePlate = command.LicensePlate,
            Type = command.Type,
            SagaId = command.SagaId
        };
    }

    protected override async Task HandlerMessageAsync(CreateVehiclesCommand command,
        CancellationToken cancellationToken = default)
    {
        var entity = new Command
        {
            SagaId = command.SagaId,
            ActionType = Entities.Types.ActionType.Create,
            SerializerType = Lib.Types.SerializerType.MessagePack,
            EntityType = Entities.Types.EntityType.Vehicles,
            Type = typeof(CreateVehiclesEvent).Name,
            Data = await _serializer.SerializeAsync(CreateEventToPublish(command))
        };

        await _service.CreateAsync(entity, cancellationToken);
    }
}
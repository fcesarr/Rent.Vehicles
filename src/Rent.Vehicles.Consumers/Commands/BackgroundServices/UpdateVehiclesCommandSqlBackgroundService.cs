using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Entities;
using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Consumers.Handlers.BackgroundServices;
using LanguageExt.Common;
using Rent.Vehicles.Consumers.Interfaces;

namespace Rent.Vehicles.Consumers.Commands.BackgroundServices;

public class UpdateVehiclesCommandSqlBackgroundService : HandlerCommandServicePublishEventBackgroundService<
    UpdateVehiclesCommand,
    UpdateVehiclesEvent,
    IDataService<Command>>
{
    public UpdateVehiclesCommandSqlBackgroundService(ILogger<UpdateVehiclesCommandSqlBackgroundService> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IDataService<Command> service) : base(logger, channel, periodicTimer, serializer, publisher, service)
    {
    }

    protected override UpdateVehiclesEvent CreateEventToPublish(UpdateVehiclesCommand command)
    {
        return new UpdateVehiclesEvent
        {
            Id = command.Id, 
            LicensePlate = command.LicensePlate,
            SagaId = command.SagaId
        };
    }

    protected override async Task<Result<Task>> HandlerMessageAsync(UpdateVehiclesCommand command, CancellationToken cancellationToken = default)
    {
        var entity = new Command
        {
            SagaId = command.SagaId,
            ActionType = Entities.Types.ActionType.Update,
            SerializerType = Lib.Types.SerializerType.MessagePack,
            EntityType = Entities.Types.EntityType.Vehicles,
            Type = typeof(DeleteVehiclesEvent).Name,
            Data = await _serializer.SerializeAsync(CreateEventToPublish(command))
        };

        return (await _service.CreateAsync(entity, cancellationToken))
            .Match(entity => Task.CompletedTask, exception => new Result<Task>(exception));
    }
}
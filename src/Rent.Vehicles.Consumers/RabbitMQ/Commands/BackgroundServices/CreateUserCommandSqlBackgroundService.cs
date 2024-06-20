using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Entities;
using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;
using LanguageExt.Common;

namespace Rent.Vehicles.Consumers.RabbitMQ.Commands.BackgroundServices;

public class CreateUserCommandSqlBackgroundService : HandlerCommandServicePublishEventBackgroundService<
    CreateUserCommand,
    CreateUserEvent,
    Command,
    IService<Command>>
{
    public CreateUserCommandSqlBackgroundService(ILogger<CreateUserCommandSqlBackgroundService> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IService<Command> service) : base(logger, channel, periodicTimer, serializer, publisher, service)
    {
    }

    protected override CreateUserEvent CreateEventToPublish(CreateUserCommand command)
    {
        return new CreateUserEvent
        {
            Id = command.Id,
            Name = command.Name,
            Number = command.Number,
            Birthday = command.Birthday,
            LicenseNumber = command.LicenseNumber,
            LicenseImage = command.LicenseImage,
            SagaId = command.SagaId
        };
    }

    protected override async Task<Result<Task>> HandlerMessageAsync(CreateUserCommand command,
        CancellationToken cancellationToken = default)
    {
        var entity = new Command
        {
            SagaId = command.SagaId,
            ActionType = Entities.Types.ActionType.Create,
            SerializerType = Lib.Types.SerializerType.MessagePack,
            EntityType = Entities.Types.EntityType.User,
            Type = typeof(CreateUserEvent).Name,
            Data = await _serializer.SerializeAsync(CreateEventToPublish(command))
        };

        return (await _service.CreateAsync(entity, cancellationToken))
            .Match(entity => Task.CompletedTask, exception => new Result<Task>(exception));
    }
}
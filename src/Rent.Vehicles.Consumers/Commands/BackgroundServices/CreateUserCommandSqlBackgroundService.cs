using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Entities;
using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Consumers.Handlers.BackgroundServices;
using Rent.Vehicles.Consumers.Interfaces;
using Rent.Vehicles.Services;
using Rent.Vehicles.Services.DataServices.Interfaces;

namespace Rent.Vehicles.Consumers.Commands.BackgroundServices;

public class CreateUserCommandSqlBackgroundService : HandlerCommandPublishEventBackgroundService<
    CreateUserCommand,
    CreateUserEvent>
{
    public CreateUserCommandSqlBackgroundService(ILogger<CreateUserCommandSqlBackgroundService> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IServiceScopeFactory serviceScopeFactory) : base(logger, channel, periodicTimer, serializer, publisher, serviceScopeFactory)
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
        var service = _serviceScopeFactory.CreateScope()
            .ServiceProvider
            .GetRequiredService<ICommandDataService>();

        var entity = new Command
        {
            SagaId = command.SagaId,
            ActionType = Entities.Types.ActionType.Create,
            SerializerType = Lib.Types.SerializerType.MessagePack,
            EntityType = Entities.Types.EntityType.User,
            Type = typeof(CreateUserEvent).Name,
            Data = await _serializer.SerializeAsync(CreateEventToPublish(command))
        };
        
        return service.CreateAsync(entity, cancellationToken);
    }
}
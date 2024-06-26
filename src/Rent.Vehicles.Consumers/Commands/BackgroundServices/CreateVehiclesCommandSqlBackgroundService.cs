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
using Rent.Vehicles.Services.Facades.Interfaces;
using Rent.Vehicles.Services.Repositories.Interfaces;

namespace Rent.Vehicles.Consumers.Commands.BackgroundServices;

public class CreateVehiclesCommandSqlBackgroundService : HandlerCommandPublishEventBackgroundService<
    CreateVehiclesCommand,
    CreateVehiclesEvent>
{
    public CreateVehiclesCommandSqlBackgroundService(ILogger<CreateVehiclesCommandSqlBackgroundService> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IServiceScopeFactory serviceScopeFactory) : base(logger, channel, periodicTimer, serializer, publisher, serviceScopeFactory)
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

    protected override async Task<Result<Task>> HandlerMessageAsync(CreateVehiclesCommand command,
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
            EntityType = Entities.Types.EntityType.Vehicles,
            Type = typeof(CreateVehiclesEvent).Name,
            Data = await _serializer.SerializeAsync(CreateEventToPublish(command))
        };
        
        return service.CreateAsync(entity, cancellationToken);
    }
}
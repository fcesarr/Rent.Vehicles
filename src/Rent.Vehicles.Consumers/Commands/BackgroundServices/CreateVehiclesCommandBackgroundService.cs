using Rent.Vehicles.Consumers.Handlers.BackgroundServices;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Entities;
using Rent.Vehicles.Entities.Types;
using Rent.Vehicles.Lib.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Lib.Types;
using Rent.Vehicles.Messages.Commands;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Services;
using Rent.Vehicles.Services.DataServices.Interfaces;
using Rent.Vehicles.Services.Facades.Interfaces;

namespace Rent.Vehicles.Consumers.Commands.BackgroundServices;

public class CreateVehiclesCommandBackgroundService : HandlerCommandPublishEventBackgroundService<
    CreateVehiclesCommand,
    CreateVehiclesEvent>
{
    public CreateVehiclesCommandBackgroundService(ILogger<CreateVehiclesCommandBackgroundService> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IServiceScopeFactory serviceScopeFactory) : base(logger, channel, periodicTimer, serializer, publisher,
        serviceScopeFactory)
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
        using var serviceScope = _serviceScopeFactory.CreateScope();

        var serviceProvider = serviceScope.ServiceProvider;
            
        var service = serviceProvider.GetRequiredService<ICommandFacade>();

        var @event = CreateEventToPublish(command);

        var entity = await service.CreateAsync(command, @event, ActionType.Create,
            EntityType.Vehicles, @event.GetType().ToString(), cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception!;
        }

        return Task.CompletedTask;
    }
}

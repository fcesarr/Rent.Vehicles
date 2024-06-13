using Rent.Vehicles.Entities;
using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;
using Rent.Vehicles.Messages.Events;

namespace Rent.Vehicles.Consumers.RabbitMQ.Events.BackgroundServices;

public class CreateVehiclesEventBackgroundService : HandlerConsumerEventToEntityBackgroundService<CreateVehiclesEvent, Vehicle>
{
    protected readonly ICreateService<Vehicle> _createService;

    public CreateVehiclesEventBackgroundService(ILogger<CreateVehiclesEventBackgroundService> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        ICreateService<Vehicle> createService,
        ICreateService<Event> createEventService) : base(logger, channel, periodicTimer, serializer, "CreateVehiclesEvent", createEventService)
    {
        _createService = createService;
    }

    protected override async Task<Vehicle> EventToEntityAsync(CreateVehiclesEvent message, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() => new Vehicle
        {
            Id = message.Id,
            Year = message.Year,
            Model = message.Model,
            LicensePlate = message.LicensePlate,
            Type = message.Type
        }, cancellationToken);
    }

    protected override async Task HandlerAsync(Vehicle entity, CancellationToken cancellationToken = default)
    {
        await _createService.CreateAsync(entity, cancellationToken);
    }
}
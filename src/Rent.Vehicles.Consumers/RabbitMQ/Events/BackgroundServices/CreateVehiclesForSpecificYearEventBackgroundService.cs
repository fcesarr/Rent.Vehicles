using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Interfaces;

namespace Rent.Vehicles.Consumers.RabbitMQ.Events.BackgroundServices;

public class CreateVehiclesForSpecificYearEventBackgroundService : HandlerConsumerEventToEntityBackgroundService<CreateVehiclesForSpecificYearEvent, VehiclesForSpecificYear>
{
    protected readonly ICreateService<VehiclesForSpecificYear> _createService;

    public CreateVehiclesForSpecificYearEventBackgroundService(ILogger<CreateVehiclesForSpecificYearEventBackgroundService> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        ICreateService<VehiclesForSpecificYear> createService,
        ICreateService<Event> createEventService) : base(logger, channel, periodicTimer, serializer, "CreateVehiclesForSpecificYearEvent", createEventService)
    {
        _createService = createService;
    }

    protected override async Task<VehiclesForSpecificYear> EventToEntityAsync(CreateVehiclesForSpecificYearEvent message, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() => new VehiclesForSpecificYear
        {
            Id = message.Id,
            Year = message.Year,
            Model = message.Model,
            LicensePlate = message.LicensePlate,
            Type = message.Type
        }, cancellationToken);
    }

    protected override async Task HandlerAsync(VehiclesForSpecificYear entity, CancellationToken cancellationToken = default)
    {
        await _createService.CreateAsync(entity, cancellationToken);
    }
}
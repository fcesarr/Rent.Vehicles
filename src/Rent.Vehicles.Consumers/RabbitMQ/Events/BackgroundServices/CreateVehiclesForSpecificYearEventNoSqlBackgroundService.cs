using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Interfaces;

namespace Rent.Vehicles.Consumers.RabbitMQ.Events.BackgroundServices;

public class CreateVehiclesForSpecificYearEventNoSqlBackgroundService : HandlerMessageAndActionBackgroundService<
    CreateVehiclesForSpecificYearEvent,
    VehiclesForSpecificYear,
    INoSqlService<VehiclesForSpecificYear>>
{
    public CreateVehiclesForSpecificYearEventNoSqlBackgroundService(ILogger<CreateVehiclesForSpecificYearEventNoSqlBackgroundService> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        INoSqlService<VehiclesForSpecificYear> service) : base(logger, channel, periodicTimer, serializer, service)
    {
    }

    protected override async Task HandlerAsync(CreateVehiclesForSpecificYearEvent @event, CancellationToken cancellationToken = default)
    {
        await _service.CreateAsync(new VehiclesForSpecificYear
        {
            Id = @event.Id,
            Year = @event.Year,
            Model = @event.Model,
            LicensePlate = @event.LicensePlate,
            Type = @event.Type
        }, cancellationToken);
    }
}
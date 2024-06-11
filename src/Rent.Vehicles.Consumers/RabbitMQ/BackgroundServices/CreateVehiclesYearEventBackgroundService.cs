using Rent.Vehicles.Entities;
using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Consumers.RabbitMQ.BackgroundServices.Abstracts;
using Rent.Vehicles.Messages.Events;

namespace Rent.Vehicles.Consumers.RabbitMQ.BackgroundServices;

public class CreateVehiclesYearEventBackgroundService : HandlerConsumerEventBackgroundService<CreateVehiclesEvent, Vehicle>
{
    public CreateVehiclesYearEventBackgroundService(ILogger<CreateVehiclesYearEventBackgroundService> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer) : base(logger, channel, periodicTimer, serializer, "CreateVehiclesYearEvent")
    {
    }

    protected override async Task HandlerAsync(CreateVehiclesEvent @event, CancellationToken cancellationToken = default)
    {
        await Task.Run(() => _logger.LogInformation("{obj}", @event), cancellationToken);
    }
}
using Rent.Vehicles.Consumers.Handlers.BackgroundServices;
using Rent.Vehicles.Consumers.Interfaces;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Entities.Projections;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Messages.Types;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Services;
using Rent.Vehicles.Services.DataServices.Interfaces;

namespace Rent.Vehicles.Consumers.Events.BackgroundServices;

public class CreateVehiclesProjectionEventBackgroundService : HandlerEventServicePublishBackgroundService<
    CreateVehiclesProjectionEvent>
{
    public CreateVehiclesProjectionEventBackgroundService(
        ILogger<CreateVehiclesProjectionEventBackgroundService> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IServiceScopeFactory serviceScopeFactory) : base(logger, channel, periodicTimer, serializer, publisher,
        serviceScopeFactory)
    {
    }

    protected override async Task<Result<Task>> HandlerMessageAsync(CreateVehiclesProjectionEvent @event,
        CancellationToken cancellationToken = default)
    {
        IVehicleProjectionDataService _service = _serviceScopeFactory.CreateScope().ServiceProvider
            .GetRequiredService<IVehicleProjectionDataService>();

        Result<VehicleProjection> entity = await _service.CreateAsync(new VehicleProjection
        {
            Id = @event.Id,
            Year = @event.Year,
            Model = @event.Model,
            LicensePlate = @event.LicensePlate,
            Type = @event.Type switch
            {
                VehicleType.B => Entities.Types.VehicleType.B,
                VehicleType.C => Entities.Types.VehicleType.C,
                VehicleType.D => Entities.Types.VehicleType.D,
                VehicleType.E => Entities.Types.VehicleType.E,
                VehicleType.A or _ => Entities.Types.VehicleType.A
            }
        }, cancellationToken);

        if (!entity.IsSuccess)
        {
            return entity.Exception;
        }

        return Task.CompletedTask;
    }
}
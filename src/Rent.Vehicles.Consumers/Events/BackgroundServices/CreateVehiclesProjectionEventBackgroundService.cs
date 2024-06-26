using Rent.Vehicles.Entities;
using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Consumers.Handlers.BackgroundServices;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Entities.Projections;
using Rent.Vehicles.Services.DataServices.Interfaces;
using Rent.Vehicles.Consumers.Interfaces;
using Rent.Vehicles.Services;

namespace Rent.Vehicles.Consumers.Events.BackgroundServices;

public class CreateVehiclesProjectionEventBackgroundService : HandlerEventServicePublishBackgroundService<
    CreateVehiclesProjectionEvent>
{
    public CreateVehiclesProjectionEventBackgroundService(ILogger<CreateVehiclesProjectionEventBackgroundService> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IServiceScopeFactory serviceScopeFactory) : base(logger, channel, periodicTimer, serializer, publisher, serviceScopeFactory)
    {
    }

    protected override async Task<Result<Task>> HandlerMessageAsync(CreateVehiclesProjectionEvent @event, CancellationToken cancellationToken = default)
    {
        var _service = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IVehicleProjectionDataService>();

        var entity = await _service.CreateAsync(new VehicleProjection
        {
            Id = @event.Id,
            Year = @event.Year,
            Model = @event.Model,
            LicensePlate = @event.LicensePlate,
            Type = @event.Type switch {
                Messages.Types.VehicleType.B => Entities.Types.VehicleType.B,
                Messages.Types.VehicleType.C => Entities.Types.VehicleType.C,
                Messages.Types.VehicleType.D => Entities.Types.VehicleType.D,
                Messages.Types.VehicleType.E => Entities.Types.VehicleType.E,
                Messages.Types.VehicleType.A or _ => Entities.Types.VehicleType.A,
            }
        }, cancellationToken);

        if(!entity.IsSuccess)
            return entity.Exception;

        return Task.CompletedTask;
    }
}

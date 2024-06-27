using Rent.Vehicles.Consumers.Handlers.BackgroundServices;
using Rent.Vehicles.Consumers.Interfaces;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Entities;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Messages.Types;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Services;
using Rent.Vehicles.Services.Interfaces;

using Event = Rent.Vehicles.Messages.Event;

namespace Rent.Vehicles.Consumers.Events.BackgroundServices;

public class CreateVehiclesEventBackgroundService : HandlerEventServicePublishEventBackgroundService<
    CreateVehiclesEvent>
{
    public CreateVehiclesEventBackgroundService(ILogger<CreateVehiclesEventBackgroundService> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IServiceProvider serviceProvider,
        IPublisher publisher,
        IServiceScopeFactory serviceScopeFactory) : base(logger, channel, periodicTimer, serializer, publisher,
        serviceScopeFactory)
    {
    }

    protected override IEnumerable<Event> CreateEventToPublish(CreateVehiclesEvent @event)
    {
        return
        [
            new CreateVehiclesProjectionEvent
            {
                Id = @event.Id,
                Year = @event.Year,
                Model = @event.Model,
                LicensePlate = @event.LicensePlate,
                Type = @event.Type,
                SagaId = @event.SagaId
            },
            new CreateVehiclesForSpecificYearEvent
            {
                Id = @event.Id,
                Year = @event.Year,
                Model = @event.Model,
                LicensePlate = @event.LicensePlate,
                Type = @event.Type,
                SagaId = @event.SagaId
            }
        ];
    }

    protected override async Task<Result<Task>> HandlerMessageAsync(CreateVehiclesEvent @event,
        CancellationToken cancellationToken = default)
    {
        IVehicleDataService _service = _serviceScopeFactory.CreateScope().ServiceProvider
            .GetRequiredService<IVehicleDataService>();

        Result<Vehicle> entity = await _service.CreateAsync(new Vehicle
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
            return entity.Exception!;
        }

        return Task.CompletedTask;
    }
}
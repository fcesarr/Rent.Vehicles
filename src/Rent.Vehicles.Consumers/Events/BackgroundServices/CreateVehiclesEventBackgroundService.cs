using Rent.Vehicles.Entities;
using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Consumers.Handlers.BackgroundServices;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Producers.Interfaces;
using LanguageExt.Common;
using Rent.Vehicles.Consumers.Interfaces;

namespace Rent.Vehicles.Consumers.Events.BackgroundServices;

public class CreateVehiclesEventBackgroundService : HandlerEventServicePublishEventBackgroundService<
    CreateVehiclesEvent,
    IVehicleDataService>
{
    public CreateVehiclesEventBackgroundService(ILogger<CreateVehiclesEventBackgroundService> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IVehicleDataService service) : base(logger, channel, periodicTimer, serializer, publisher, service)
    {
    }

    protected override IEnumerable<Messages.Event> CreateEventToPublish(CreateVehiclesEvent @event)
    {
        return [
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

    protected override async Task<Result<Task>> HandlerMessageAsync(CreateVehiclesEvent @event, CancellationToken cancellationToken = default)
    {
        var entity = await _service.CreateAsync(new Vehicle
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
            return new Result<Task>(entity.Exception);

        return Task.CompletedTask;
    }
}

using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Entities;
using Rent.Vehicles.Services.Interfaces;
using LanguageExt.Common;
using Rent.Vehicles.Entities.Projections;

namespace Rent.Vehicles.Consumers.RabbitMQ.Events.BackgroundServices;

public class CreateVehiclesForSpecificYearEventNoSqlBackgroundService : HandlerEventServicePublishBackgroundService<
    CreateVehiclesForSpecificYearEvent,
    IDataService<VehiclesForSpecificYearProjection>>
{
    public CreateVehiclesForSpecificYearEventNoSqlBackgroundService(ILogger<CreateVehiclesForSpecificYearEventNoSqlBackgroundService> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IDataService<VehiclesForSpecificYearProjection> service) : base(logger, channel, periodicTimer, serializer, publisher, service)
    {
    }

    protected override async Task<Result<Task>> HandlerMessageAsync(CreateVehiclesForSpecificYearEvent @event, CancellationToken cancellationToken = default)
    {
        var entity = await _service.CreateAsync(new VehiclesForSpecificYearProjection
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

        return entity.Match(entity => Task.CompletedTask, exception => new Result<Task>(exception));
    }
}
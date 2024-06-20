using Rent.Vehicles.Entities;
using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Producers.Interfaces;
using LanguageExt.Common;

namespace Rent.Vehicles.Consumers.RabbitMQ.Events.BackgroundServices;

public class CreateVehiclesEventSqlBackgroundService : HandlerEventServicePublishEventBackgroundService<
    CreateVehiclesEvent,
    CreateVehiclesSuccessEvent,
    Vehicle,
    IVehicleService>
{
    public CreateVehiclesEventSqlBackgroundService(ILogger<CreateVehiclesEventSqlBackgroundService> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IVehicleService service) : base(logger, channel, periodicTimer, serializer, publisher, service)
    {
    }

    protected override CreateVehiclesSuccessEvent CreateEventToPublish(CreateVehiclesEvent @event)
    {
        return new CreateVehiclesSuccessEvent
        {
            Id = @event.Id,
            Year = @event.Year,
            Model = @event.Model,
            LicensePlate = @event.LicensePlate,
            Type = @event.Type,
            SagaId = @event.SagaId
        };
    }

    protected override async Task<Result<Task>> HandlerMessageAsync(CreateVehiclesEvent @event, CancellationToken cancellationToken = default)
    {
        var entity = await _service.CreateAsync(new Vehicle
        {
            Id = @event.Id,
            Year = @event.Year,
            Model = @event.Model,
            LicensePlate = @event.LicensePlate,
            Type = @event.Type
        }, cancellationToken);

        return entity.Match(entity => Task.CompletedTask, exception => new Result<Task>(exception));
    }

    protected override async Task PublishAsync(CreateVehiclesSuccessEvent @event, CancellationToken cancellationToken = default)
    {
        await _publisher.PublishEventAsync(@event, cancellationToken);
    }
}

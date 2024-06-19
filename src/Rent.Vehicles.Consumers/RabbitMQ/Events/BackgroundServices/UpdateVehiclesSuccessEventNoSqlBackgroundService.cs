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

public class UpdateVehiclesSuccessEventNoSqlBackgroundService : HandlerEventServicePublishBackgroundService<
    UpdateVehiclesSuccessEvent,
    Vehicle,
    INoSqlVehiclesService>
{
    public UpdateVehiclesSuccessEventNoSqlBackgroundService(ILogger<UpdateVehiclesSuccessEventNoSqlBackgroundService> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        INoSqlVehiclesService service) : base(logger, channel, periodicTimer, serializer, publisher, service)
    {
    }

    protected override async Task<Result<Task>> HandlerMessageAsync(UpdateVehiclesSuccessEvent @event, CancellationToken cancellationToken = default)
    {
        var entity = await _service.UpdateAsync(@event.Id, @event.LicensePlate, cancellationToken);

        return entity.Match(entity => Task.CompletedTask, exception => new Result<Task>(exception));
    }
}

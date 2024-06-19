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

public class CreateVehiclesSuccessEventNoSqlBackgroundService : HandlerEventServicePublishBackgroundService<
    CreateVehiclesSuccessEvent,
    Vehicle,
    IVehiclesService>
{
    public CreateVehiclesSuccessEventNoSqlBackgroundService(ILogger<CreateVehiclesSuccessEventNoSqlBackgroundService> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IVehiclesService service) : base(logger, channel, periodicTimer, serializer, publisher, service)
    {
        QueueName = $"{typeof(CreateVehiclesSuccessEvent).Name}.One";
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        var result = base.StartAsync(cancellationToken);

        _channel.QueueBind(queue: QueueName,
            exchange: typeof(CreateVehiclesSuccessEvent).Name, routingKey:"");
        
        return result;
    }

    protected override async Task<Result<Task>> HandlerMessageAsync(CreateVehiclesSuccessEvent @event, CancellationToken cancellationToken = default)
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
}

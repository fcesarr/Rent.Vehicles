using Rent.Vehicles.Entities;
using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Consumers.Exceptions;

namespace Rent.Vehicles.Consumers.RabbitMQ.Events.BackgroundServices;

public class CreateVehiclesSuccessEventSpecificYearBackgroundService : HandlerEventPublishEventBackgroundService<
    CreateVehiclesSuccessEvent,
    CreateVehiclesForSpecificYearEvent>
{
    public CreateVehiclesSuccessEventSpecificYearBackgroundService(ILogger<CreateVehiclesSuccessEventSpecificYearBackgroundService> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher) : base(logger, channel, periodicTimer, serializer, publisher)
    {
        QueueName = $"{typeof(CreateVehiclesSuccessEvent).Name}.Two";
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
        var result = base.StartAsync(cancellationToken);

        _channel.QueueBind(queue: QueueName,
            exchange: typeof(CreateVehiclesSuccessEvent).Name, routingKey:"");
        
        return result;
    }

    protected override CreateVehiclesForSpecificYearEvent CreateEventToPublish(CreateVehiclesSuccessEvent @event)
    {
        return new CreateVehiclesForSpecificYearEvent
        {
            Id = @event.Id,
            Year = @event.Year,
            Model = @event.Model,
            LicensePlate = @event.LicensePlate,
            Type = @event.Type,
            SagaId = @event.SagaId
        };
    }

    protected override Task HandlerMessageAsync(CreateVehiclesSuccessEvent command, CancellationToken cancellationToken = default)
    {
        if(command.Year != 2024)
            throw new SpecificYearException();
        
        return Task.CompletedTask;
    }
}

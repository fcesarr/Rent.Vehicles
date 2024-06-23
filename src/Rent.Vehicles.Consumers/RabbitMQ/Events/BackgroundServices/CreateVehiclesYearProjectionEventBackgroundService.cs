using Rent.Vehicles.Entities;
using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Consumers.Exceptions;
using LanguageExt.Common;
using LanguageExt;
using Rent.Vehicles.Messages;

namespace Rent.Vehicles.Consumers.RabbitMQ.Events.BackgroundServices;

public class CreateVehiclesYearProjectionEventBackgroundService : HandlerEventPublishEventBackgroundService<
    CreateVehiclesYearProjectionEvent>
{
    public CreateVehiclesYearProjectionEventBackgroundService(ILogger<CreateVehiclesYearProjectionEventBackgroundService> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher) : base(logger, channel, periodicTimer, serializer, publisher)
    {
    }

    protected override IEnumerable<Messages.Event> CreateEventToPublish(CreateVehiclesYearProjectionEvent @event)
    {
        return [
            new CreateVehiclesForSpecificYearProjectionEvent
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

    protected override Task<Result<Task>> HandlerMessageAsync(CreateVehiclesYearProjectionEvent @event, CancellationToken cancellationToken = default)
    {
        var result = new Result<Task>(Task.CompletedTask);
        
        if(@event.Year != 2024)
            result = new Result<Task>(new SpecificYearException(string.Empty));
        
        return Task.FromResult(result);
    }
}

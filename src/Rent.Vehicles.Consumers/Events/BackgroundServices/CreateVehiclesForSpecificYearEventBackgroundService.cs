using Rent.Vehicles.Consumers.Exceptions;
using Rent.Vehicles.Consumers.Handlers.BackgroundServices;
using Rent.Vehicles.Consumers.Interfaces;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Messages.Projections.Events;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Services;

using Event = Rent.Vehicles.Messages.Event;

namespace Rent.Vehicles.Consumers.Events.BackgroundServices;

public class CreateVehiclesForSpecificYearEventBackgroundService : HandlerEventPublishEventBackgroundService<
    CreateVehiclesForSpecificYearEvent>
{
    public CreateVehiclesForSpecificYearEventBackgroundService(
        ILogger<CreateVehiclesForSpecificYearEventBackgroundService> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher) : base(logger, channel, periodicTimer, serializer, publisher)
    {
    }

    protected override IEnumerable<Event> CreateEventToPublish(CreateVehiclesForSpecificYearEvent @event)
    {
        return
        [
            new CreateVehiclesForSpecificYearProjectionEvent
            {
                Id = @event.Id,
                SagaId = @event.SagaId
            }
        ];
    }

    protected override Task<Result<Task>> HandlerMessageAsync(CreateVehiclesForSpecificYearEvent @event,
        CancellationToken cancellationToken = default)
    {
        if (@event.Year != 2024)
        {
            return Task.Run(() => Result<Task>.Failure(new SpecificYearException(string.Empty)), cancellationToken);
        }

        return Task.Run(() => Result<Task>.Success(Task.CompletedTask), cancellationToken);
    }
}

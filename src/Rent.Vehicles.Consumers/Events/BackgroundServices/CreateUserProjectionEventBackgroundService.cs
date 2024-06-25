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

public class CreateUserProjectionEventBackgroundService : HandlerEventServicePublishBackgroundService<
    CreateUserProjectionEvent,
    IVehicleProjectionDataService>
{
    public CreateUserProjectionEventBackgroundService(ILogger<CreateUserProjectionEventBackgroundService> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IVehicleProjectionDataService service) : base(logger, channel, periodicTimer, serializer, publisher, service)
    {
    }

    protected override Task<Result<Task>> HandlerMessageAsync(CreateUserProjectionEvent @event, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => Result<Task>.Failure(new Exception()), cancellationToken);
    }
}

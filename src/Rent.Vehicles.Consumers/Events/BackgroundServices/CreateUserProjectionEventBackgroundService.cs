using Rent.Vehicles.Consumers.Handlers.BackgroundServices;
using Rent.Vehicles.Consumers.Interfaces;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Services;
using Rent.Vehicles.Services.DataServices.Interfaces;

namespace Rent.Vehicles.Consumers.Events.BackgroundServices;

public class CreateUserProjectionEventBackgroundService : HandlerEventServicePublishBackgroundService<
    CreateUserProjectionEvent>
{
    public CreateUserProjectionEventBackgroundService(ILogger<CreateUserProjectionEventBackgroundService> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IServiceProvider serviceProvider,
        IPublisher publisher,
        IServiceScopeFactory serviceScopeFactory) : base(logger, channel, periodicTimer, serializer, publisher,
        serviceScopeFactory)
    {
    }

    protected override Task<Result<Task>> HandlerMessageAsync(CreateUserProjectionEvent @event,
        CancellationToken cancellationToken = default)
    {
        IVehicleProjectionDataService _service = _serviceScopeFactory.CreateScope().ServiceProvider
            .GetRequiredService<IVehicleProjectionDataService>();

        return Task.Run(() => Result<Task>.Failure(new Exception()), cancellationToken);
    }
}
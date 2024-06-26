using Rent.Vehicles.Entities;
using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Consumers.Handlers.BackgroundServices;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Producers.Interfaces;
using Rent.Vehicles.Services.Facades.Interfaces;
using Rent.Vehicles.Consumers.Interfaces;
using Rent.Vehicles.Services;

namespace Rent.Vehicles.Consumers.Events.BackgroundServices;

public class CreateUserEventBackgroundService : HandlerEventServicePublishEventBackgroundService<
    CreateUserEvent>
{
    public CreateUserEventBackgroundService(ILogger<CreateUserEventBackgroundService> logger,
        IConsumer channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IServiceScopeFactory serviceScopeFactory) : base(logger, channel, periodicTimer, serializer, publisher, serviceScopeFactory)
    {
    }

    protected override IEnumerable<Messages.Event> CreateEventToPublish(CreateUserEvent @event)
    {
        return [
            new CreateUserProjectionEvent
            {
                Id = @event.Id,
                Name = @event.Name,
                Number = @event.Number,
                Birthday = @event.Birthday,
                LicenseNumber = @event.LicenseNumber,
                LicenseImage = @event.LicenseImage,
                SagaId = @event.SagaId
            },
            new UploadUserLicenseImageEvent
            {
                LicenseImage = @event.LicenseImage,
                SagaId = @event.SagaId
            }
        ];
    }

    protected override async Task<Result<Task>> HandlerMessageAsync(CreateUserEvent @event, CancellationToken cancellationToken = default)
    {
        var _service = _serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IUserFacade>();

        var user = await _service.CreateAsync(@event, cancellationToken);

        if(!user.IsSuccess)
            return user.Exception!;

        return Task.CompletedTask;
    }
}
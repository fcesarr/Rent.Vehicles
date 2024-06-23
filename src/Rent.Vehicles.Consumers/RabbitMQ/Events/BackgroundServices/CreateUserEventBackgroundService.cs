using Rent.Vehicles.Entities;
using RabbitMQ.Client;
using Rent.Vehicles.Consumers.Utils.Interfaces;
using Rent.Vehicles.Lib.Serializers.Interfaces;
using Rent.Vehicles.Services.Interfaces;
using Rent.Vehicles.Consumers.RabbitMQ.Handlers.BackgroundServices;
using Rent.Vehicles.Messages.Events;
using Rent.Vehicles.Producers.Interfaces;
using LanguageExt.Common;
using Rent.Vehicles.Services.Facades.Interfaces;

namespace Rent.Vehicles.Consumers.RabbitMQ.Events.BackgroundServices;

public class CreateUserEventBackgroundService : HandlerEventServicePublishEventBackgroundService<
    CreateUserEvent,
    IUserFacade>
{
    public CreateUserEventBackgroundService(ILogger<CreateUserEventBackgroundService> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IUserFacade userFacade) : base(logger, channel, periodicTimer, serializer, publisher, userFacade)
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
        var entity = await _service.CreateAsync(@event, cancellationToken);

        return entity.Match(entity => Task.CompletedTask, exception => new Result<Task>(exception));
    }
}

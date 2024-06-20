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

public class CreateUserEventSqlBackgroundService : HandlerEventServicePublishEventBackgroundService<
    CreateUserEvent,
    CreateUserSuccessEvent,
    User,
    IUserService>
{
    public CreateUserEventSqlBackgroundService(ILogger<CreateUserEventSqlBackgroundService> logger,
        IModel channel,
        IPeriodicTimer periodicTimer,
        ISerializer serializer,
        IPublisher publisher,
        IUserService service) : base(logger, channel, periodicTimer, serializer, publisher, service)
    {
    }

    protected override CreateUserSuccessEvent CreateEventToPublish(CreateUserEvent @event)
    {
        return new CreateUserSuccessEvent
        {
            Id = @event.Id,
            Name = @event.Name,
            Number = @event.Number,
            Birthday = @event.Birthday,
            LicenseNumber = @event.LicenseNumber,
            LicenseImage = @event.LicenseImage,
            SagaId = @event.SagaId
        };
    }

    protected override async Task<Result<Task>> HandlerMessageAsync(CreateUserEvent @event, CancellationToken cancellationToken = default)
    {
        var entity = await _service.CreateAsync(new User
        {
            Id = @event.Id,
            Name = @event.Name,
            Number = @event.Number,
            Birthday = @event.Birthday,
            LicenseNumber = @event.LicenseNumber,
            LicensePath = @event.LicenseImage,
        }, cancellationToken);

        return entity.Match(entity => Task.CompletedTask, exception => new Result<Task>(exception));
    }
}
